using DomainDrivenDesign.Api.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DomainDrivenDesign.Api.Data;

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
        options.UseNpgsql(configuration.GetConnectionString("ApiAwsConnectionString"), b => b.MigrationsAssembly("DomainDrivenDesign.Api.WebApplication"));
    }

    public DbSet<RecipeProfile> RecipeProfiles { get; set; }
}
