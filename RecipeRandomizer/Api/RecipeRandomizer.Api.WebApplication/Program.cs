using System.Text;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Domain.Commands;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using NSwag.Generation.Processors.Security;
using RecipeRandomizer.Api.Domain.Proxies;
using GoogleClient = GoogleCustomSearchService.Api.Client.GoogleCustomSearchClient;
using Serilog.Sinks.AwsCloudWatch;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Amazon;
using RecipeRandomizer.Api.WebApplication.ExceptionHandler;
using Utilities.ConfigurationManager.Extensions;
using MassTransit;

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

builder.Services.AddMassTransit(x => 
{
    x.UsingRabbitMq();
});

builder.Services.AddHttpClient("GoogleCustomSearchClient", config => 
{
    config.BaseAddress = new Uri(builder.Configuration.GetStringValue("GoogleCustomSearchClient:Url"));
});

builder.Services.AddSingleton(c => 
{
    var factory = c.GetService<IHttpClientFactory>();
    var httpClient = factory?.CreateClient("GoogleCustomSearchClient");

    ArgumentNullException.ThrowIfNull(httpClient);

    httpClient.BaseAddress = new Uri(builder.Configuration.GetStringValue("GoogleCustomSearchClient:Url"));
    
    return new GoogleClient(httpClient);
});

builder.Services.AddTransient<IGoogleCustomSearchServiceProxy, GoogleCustomSearchServiceProxy>();

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

var app = builder.Build();

if(builder.Configuration.GetBoolValue("AwsCloudwatchLogging:Enabled") == true)
{
    var client = new AmazonCloudWatchLogsClient(new BasicAWSCredentials(builder.Configuration.GetStringValue("AwsCloudwatchLogging:AccessKey"), builder.Configuration.GetStringValue("AwsCloudwatchLogging:SecretKey")), RegionEndpoint.USEast1);

    Log.Logger = new LoggerConfiguration().WriteTo.AmazonCloudWatch(
        logGroup: builder.Configuration.GetStringValue("AwsCloudwatchLogging:LogGroup"),
        logStreamPrefix: builder.Configuration.GetStringValue("AwsCloudwatchLogging:LogStreamPrefix"),
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

app.Run();
