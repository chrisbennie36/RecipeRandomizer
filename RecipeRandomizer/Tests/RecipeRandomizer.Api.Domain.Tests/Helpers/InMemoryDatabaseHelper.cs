namespace RecipeRandomizer.Api.Domain.Tests.Helpers;

using RecipeRandomizer.Api.Data;
using Microsoft.EntityFrameworkCore;


public static class InMemoryDatabaseHelper
{
    public static AppDbContext InitialiseDatabase()
    {
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseInMemoryDatabase(databaseName: "RecipeRandomizerTestDb");

        var dbContextOptions = builder.Options;
        AppDbContext appDbContext = new AppDbContext(dbContextOptions);
        appDbContext.Database.EnsureDeleted();
        appDbContext.Database.EnsureCreated();
        
        return appDbContext;
    }
}
