using System.Text;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Domain.Commands;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using NSwag.Generation.Processors.Security;
using Serilog.Sinks.AwsCloudWatch;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Amazon;
using RecipeRandomizer.Api.WebApplication.ExceptionHandler;
using Utilities.ConfigurationManager.Extensions;
using MassTransit;
using RecipeRandomizer.Api.Domain.EventConsumers;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Primitives;
using RecipeRandomizer.Shared.Constants;
using RecipeRandomizer.Shared.Configuration;
using Refit;
using RecipeRandomizer.Api.Domain.Clients;
using RecipeRandomizer.Api.Data.Repositories;
using RecipeRandomizer.Api.Data.Entities;
using RecipeRandomizer.Api.Data.GraphQlQueryProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMvcCore().AddApiExplorer();
//Nswag: Useful setup link for both NSwag and Swashbuckle here: https://code-maze.com/aspnetcore-swashbuckle-vs-nswag/
builder.Services.AddOpenApiDocument(config => 
{
    config.Title = "Recipe Randomizer API";
    config.AddSecurity("Bearer", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    });
    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddUserRecipePreferencesCommand).Assembly));
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();

//For more control over DBContexts, can make use of the DbContextScope approach described here: https://mehdi.me/ambient-dbcontext-in-ef6/, https://github.com/mehdime/DbContextScope?ref=mehdi.me 
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddTransient<IEntityRepository<UserRecipePreference>, UserRecipePreferencesRepository>();
builder.Services.AddTransient<UserRecipePreferencesRepository>();    //For concrete class constructor injection 

builder.Services.AddMassTransit(x => 
{
    x.AddConsumer<RecipeRatedEventConsumer>()
        .Endpoint(e => 
        {
            e.Name = "recipe-rated-event";
        });

    x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
});

var busControl = Bus.Factory.CreateUsingRabbitMq();

await busControl.StartAsync();

builder.Services.AddRefitClient<IGoogleCustomSearchClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration.GetStringValue("GoogleCustomSearchClient:Url")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
{
    options.RequireHttpsMetadata = false;   //NOTE: ONLY FOR DEVELOPMENT
    //options.Authority = "localhost:5175";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetStringValue("Jwt:Key")))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddMemoryCache();

ConcurrencyRateLimiterConfiguration concurrencyRateLimiterConfig = new ConcurrencyRateLimiterConfiguration();
builder.Configuration.GetSection(ConcurrencyRateLimiterConfiguration.Key).Bind(concurrencyRateLimiterConfig);

builder.Services.AddRateLimiter(cfg => 
{
    cfg.AddConcurrencyLimiter(policyName: RateLimiterConstants.PostRateLimiterPolicyName, options => 
    {
        options.PermitLimit = concurrencyRateLimiterConfig.PermitLimit;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = concurrencyRateLimiterConfig.QueueLimit;
    })
    .AddPolicy<string>(policyName: RateLimiterConstants.PostRateLimiterPolicyName, partitioner: (HttpContext httpContext) => 
    {
        string username = httpContext.User.Identity?.Name ?? string.Empty;

        if(!StringValues.IsNullOrEmpty(username))
        {
            return RateLimitPartition.GetTokenBucketLimiter(username, _ => 
                new TokenBucketRateLimiterOptions 
                {
                    TokenLimit = concurrencyRateLimiterConfig.AuthorizedUserTokenLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = concurrencyRateLimiterConfig.QueueLimit,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(concurrencyRateLimiterConfig.ReplenishmentPeriodSeconds),
                    TokensPerPeriod = concurrencyRateLimiterConfig.TokensPerPeriod,
                    AutoReplenishment = concurrencyRateLimiterConfig.AutoReplenishmentEnabled
                });
        }

        return RateLimitPartition.GetTokenBucketLimiter(RateLimiterConstants.AnonymousUserRateLimiterPolicyName, _ =>
            new TokenBucketRateLimiterOptions
            {
                TokenLimit = concurrencyRateLimiterConfig.AnonymousUserTokenLimit,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = concurrencyRateLimiterConfig.QueueLimit,
                ReplenishmentPeriod = TimeSpan.FromSeconds(concurrencyRateLimiterConfig.ReplenishmentPeriodSeconds),
                TokensPerPeriod = concurrencyRateLimiterConfig.TokensPerPeriod,
                AutoReplenishment = true
            });
    });
});

builder.Services.AddGraphQLServer()
    .AddQueryType<RecipeRatingQueryProvider>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

AwsLoggingConfiguration awsLoggingConfig = new AwsLoggingConfiguration();
builder.Configuration.GetSection(AwsLoggingConfiguration.Key).Bind(awsLoggingConfig);

if(awsLoggingConfig.Enabled)
{
    var client = new AmazonCloudWatchLogsClient(new BasicAWSCredentials(awsLoggingConfig.AccessKey, awsLoggingConfig.SecretKey), RegionEndpoint.USEast1);

    Log.Logger = new LoggerConfiguration().WriteTo.AmazonCloudWatch(
        logGroup: awsLoggingConfig.LogGroup,
        logStreamPrefix: awsLoggingConfig.LogStreamPrefix,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug,
        createLogGroup: true,
        appendUniqueInstanceGuid: true,
        appendHostName: false,
        logGroupRetentionPolicy: LogGroupRetentionPolicy.ThreeDays,
        cloudWatchClient: client).CreateLogger();
}
else
{
    Log.Logger = new LoggerConfiguration().WriteTo.File("./Logs/logs-", rollingInterval: RollingInterval.Day).MinimumLevel.Debug().CreateLogger();
}

if(app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseStatusCodePages();
app.UseExceptionHandler();

//The following three should be in this exact order - see here: https://stackoverflow.com/questions/43574552/authorization-in-asp-net-core-always-401-unauthorized-for-authorize-attribute
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.MapGraphQL("/graphQL");

app.Run();
