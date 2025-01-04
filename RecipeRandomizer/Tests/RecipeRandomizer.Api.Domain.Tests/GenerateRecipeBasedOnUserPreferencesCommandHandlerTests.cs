using AutoMapper;
using GoogleCustomSearchService.Api.Client;
using MediatR;
using Moq;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Handlers;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Proxies;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Api.Domain.Results;
using RecipeRandomizer.Api.WebApplication.Mapper;
using Xunit;

namespace RecipeRandomizer.Api.Domain.Tests;

public class GenerateRecipeBasedOnUserPreferencesCommandHandlerTests
{
    const int UserId = 50;

    private readonly Mock<ISender> senderMock;
    private readonly Mock<IGoogleCustomSearchServiceProxy> googleCustomSearchProxy;
    private readonly IMapper mapper;

    public GenerateRecipeBasedOnUserPreferencesCommandHandlerTestsTests()
    {
        RecipePreferenceModel recipePreferenceModel = new RecipePreferenceModel
        {
            Name = "Test",
            Type = Shared.Enums.RecipePreferenceType.Meat
        };

        List<RecipePreferenceModel> recipePreferences = new List<RecipePreferenceModel>
        {
            recipePreferenceModel
        };

        DomainResult<IEnumerable<RecipePreferenceModel>> successResult = new DomainResult<IEnumerable<RecipePreferenceModel>>(ResponseStatus.Success, recipePreferences);

        senderMock = new Mock<ISender>();
        senderMock.Setup(s => s.Send(It.IsAny<GetUserRecipePreferencesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(successResult);

        googleCustomSearchProxy = new Mock<IGoogleCustomSearchServiceProxy>();

        googleCustomSearchProxy.Setup(g => g.SearchAsync(It.IsAny<GoogleCustomSearchDto>())).ReturnsAsync(new GoogleCustomSearchResponse { Items = new List<Item>() });

        MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(DefaultProfile).Assembly));
        mapper = new Mapper(config);
    }

    [Fact]
    public async Task GenerateRecipeBasedOnUserPreferencesCommandHandlerTests_RetrievesUserRecipePreferences()
    {
        var sut = new GenerateRecipeBasedOnUserPreferencesCommandHandlerTests(senderMock.Object, mapper, googleCustomSearchProxy.Object);

        var result = await sut.Handle(new GenerateRecipeBasedOnUserPreferencesCommand(UserId), CancellationToken.None);

        senderMock.Verify(s => s.Send(It.IsAny<GetUserRecipePreferencesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateRecipeBasedOnUserPreferencesCommandHandlerTests_ReturnsEmptyResponse_IfNoUserRecipePreferences()
    {
        DomainResult<IEnumerable<RecipePreferenceModel>> emptyUserRecipePreferencesResult = new DomainResult<IEnumerable<RecipePreferenceModel>>(ResponseStatus.Success, Enumerable.Empty<RecipePreferenceModel>());

        senderMock.Setup(s => s.Send(It.IsAny<GetUserRecipePreferencesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(emptyUserRecipePreferencesResult);

        var sut = new GenerateRecipeBasedOnUserPreferencesCommandHandlerTests(senderMock.Object, mapper, googleCustomSearchProxy.Object);

        var result = await sut.Handle(new GenerateRecipeBasedOnUserPreferencesCommand(UserId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.resultModel);
        Assert.Null(result.resultModel.RecipeUrl);
        Assert.Empty(result.resultModel.ErrorTraceId);
    }

    [Fact]
    public async Task GenerateRecipeBasedOnUserPreferencesCommandHandlerTests_CallsTheGoogleCustomSearchClient_WhenUserRecipePreferencesExist()
    {
        var sut = new GenerateRecipeBasedOnUserPreferencesCommandHandlerTests(senderMock.Object, mapper, googleCustomSearchProxy.Object);

        var result = await sut.Handle(new GenerateRecipeBasedOnUserPreferencesCommand(UserId), CancellationToken.None);

        googleCustomSearchProxy.Verify(g => g.SearchAsync(It.IsAny<GoogleCustomSearchDto>()), Times.Once);
    }
}