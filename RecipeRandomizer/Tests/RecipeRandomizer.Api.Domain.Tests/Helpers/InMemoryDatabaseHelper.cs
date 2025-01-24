namespace RecipeRandomizer.Api.Domain.Tests.Helpers;

using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Infrastructure.Repositories;

public static class InMemoryDatabaseHelper
{
    public static AppDbContext InitialiseDatabase()
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseInMemoryDatabase(databaseName: "RecipeRandomizerTestDb");

        var dbContextOptions = builder.Options;
        AppDbContext appDbContext = new AppDbContext(null, dbContextOptions);
        appDbContext.Database.EnsureDeleted();
        appDbContext.Database.EnsureCreated();
        
        return appDbContext;
    }
}
