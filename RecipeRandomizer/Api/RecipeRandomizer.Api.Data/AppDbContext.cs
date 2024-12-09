using RecipeRandomizer.Api.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace RecipeRandomizer.Api.Data;

public class AppDbContext : DbContext
{
    protected readonly IConfiguration configuration;

    //public AppDbContext() : base() {}

    public AppDbContext(IConfiguration configuration) : base()
    {
        this.configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(configuration.GetConnectionString("ApiAwsConnectionString"), b => b.MigrationsAssembly("RecipeRandomizer.Api.WebApplication"));
        //options.UseNpgsql(configuration.GetConnectionString("ApiConnectionString"), b => b.MigrationsAssembly("RecipeRandomizer.Api.WebApplication"));
    }

    public DbSet<RecipePreference> RecipePreferences { get; set; }
    public DbSet<UserRecipePreference> UserRecipePreferences { get; set; }
}