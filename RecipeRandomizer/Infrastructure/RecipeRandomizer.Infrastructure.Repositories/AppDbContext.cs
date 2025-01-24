using RecipeRandomizer.Infrastructure.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace RecipeRandomizer.Infrastructure.Repositories;

public class AppDbContext : DbContext
{
    protected readonly IConfiguration configuration;

    public AppDbContext(IConfiguration configuration, DbContextOptions<AppDbContext> options) : base(options)
    {
        this.configuration = configuration;
    }

    //Uncomment when generating Migrations and commnt out the constructor above
    /*public AppDbContext()
    {}*/

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
       // options.UseNpgsql(configuration.GetConnectionString("ApiAwsConnectionString"), b => b.MigrationsAssembly("RecipeRandomizer.Api.WebApplication"));
        options.UseNpgsql(configuration.GetConnectionString("ApiConnectionString"), b => b.MigrationsAssembly("RecipeRandomizer.Api.WebApplication"));
    }

    public DbSet<RecipePreference> RecipePreferences { get; set; }
    public DbSet<UserRecipePreference> UserRecipePreferences { get; set; }
    public DbSet<UserRecipeRating> UserRecipeRatings { get; set; }
    public DbSet<UserRecipeFavourite> UserRecipeFavourites { get; set; }
}
