namespace RecipeRandomizer.Api.Domain.Tests;

using RecipeRandomizer.Api.Domain.Tests.Helpers;
using RecipeRandomizer.Api.Data;
using Xunit;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Api.Data.Entities;
using RecipeRandomizer.Api.Domain.Handlers;
using AutoMapper;
using Moq;
using Microsoft.EntityFrameworkCore;

public class GetUserRecipePreferencesQueryHandlerTests
{
    const int UserId = 1;
    const int RecipePreferenceId = 1;
    const string RecipePreferenceName = "TestRecipePreference";

    private AppDbContext appDbContext;

    public GetUserRecipePreferencesQueryHandlerTests()
    {
        appDbContext = InMemoryDatabaseHelper.InitialiseDatabase();
    }

    [Fact]
    public async Task GetUserRecipePreferencesQueryHandlerTests_ReturnsEmptyList_IfNoResultsFound()
    {
        Mock<IMapper> mapper = new Mock<IMapper>();

        var sut = new GetUserRecipePreferencesQueryHandler(appDbContext, mapper.Object);

        var result = await sut.Handle(new GetUserRecipePreferencesQuery(1), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.resultModel);
    }

       [Fact]
    public async Task GetUserRecipePreferencesQueryHandlerTests_ReturnsListOfPreferences_ForGivenUser()
    {
        Mock<IMapper> mapper = new Mock<IMapper>();

        await InitialiseDatabase();

        var sut = new GetUserRecipePreferencesQueryHandler(appDbContext, mapper.Object);

        var result = await sut.Handle(new GetUserRecipePreferencesQuery(UserId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.resultModel);

        await ClearDatabase();
    }

    private async Task InitialiseDatabase()
    {
        appDbContext.RecipePreferences.Add(new Data.Entities.RecipePreference 
        {
            Name = RecipePreferenceName,
            Type = Shared.Enums.RecipePreferenceType.Meat,
            Excluded = false,
            CreatedUtc = DateTime.UtcNow
        });

        appDbContext.UserRecipePreferences.Add(new UserRecipePreference 
        {
            UserId = UserId,
            RecipePreferenceId = RecipePreferenceId,
            CreatedUtc = DateTime.UtcNow
        });

        await appDbContext.SaveChangesAsync();
    }

    private async Task ClearDatabase()
    {
        await appDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [RecipePreferences]");
        await appDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [RecipePreferences]");

        await appDbContext.SaveChangesAsync();
    }
}
