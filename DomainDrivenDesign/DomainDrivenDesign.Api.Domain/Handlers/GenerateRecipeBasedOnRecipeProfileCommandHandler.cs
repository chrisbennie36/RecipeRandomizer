using System.Text;
using AutoMapper;
using DomainDrivenDesign.Api.Domain.Commands;
using DomainDrivenDesign.Api.Domain.Models;
using DomainDrivenDesign.Api.Domain.Queries;
using DomainDrivenDesign.Api.Domain.Results;
using DomainDrivenDesign.Api.GoogleCustomSearchClient.Interfaces;
using DomainDrivenDesign.Api.GoogleCustomSearchClient.Results;
using DomainDrivenDesign.Shared.Enums;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class GenerateRecipeBasedOnRecipeProfileCommandHandler : IRequestHandler<GenerateRecipeBasedOnRecipeProfileCommand, string>
{
    private ISender sender;
    private IMapper mapper;
    private IGoogleCustomSearchClient googleCustomSearchClient;

    public GenerateRecipeBasedOnRecipeProfileCommandHandler(ISender sender, IMapper mapper, IGoogleCustomSearchClient googleCustomSearchClient)
    {
        this.sender = sender;
        this.mapper = mapper;
        this.googleCustomSearchClient = googleCustomSearchClient;
    }

    public async Task<string> Handle(GenerateRecipeBasedOnRecipeProfileCommand request, CancellationToken cancellationToken)
    {
        RecipeProfileResult? recipeProfileResult = await sender.Send(new GetRecipeProfileByIdQuery(request.recipeProfileId));

        if(recipeProfileResult == null)
        {
            Log.Error($"Could not retrieve a {nameof(RecipeProfileResult)} when generating a recipe URL");
            return string.Empty;
        }

        string recipeQueryParams = GenerateRecipeQuery(mapper.Map<RecipeProfileModel>(recipeProfileResult));

        if(string.IsNullOrEmpty(recipeQueryParams))
        {
            Log.Error($"Could not generate recipe query params for {typeof(MeatType)}: {recipeProfileResult.MeatType}");
            return string.Empty;
        }

        GoogleCustomSearchResult? customSearchResult = await googleCustomSearchClient.GetResults(recipeQueryParams);

        if(customSearchResult == null)
        {
            return string.Empty;
        }

        return customSearchResult.Items.First().Link;
    }

    private string GenerateRecipeQuery(RecipeProfileModel recipeProfile)
    {
        StringBuilder sb = new StringBuilder("q=");

        switch(recipeProfile.MeatType)
        {
            case Shared.Enums.MeatType.Beef:
                sb.Append("beef+recipe");
                break;
            case Shared.Enums.MeatType.Pork:
                sb.Append("pork+recipe");
                break;
            case Shared.Enums.MeatType.Chicken:
                sb.Append("chicken+recipe");
                break;
            case Shared.Enums.MeatType.White:
                sb.Append("white+meat+recipe");
                break;
            case Shared.Enums.MeatType.Red:
                sb.Append("red+meat+recipe");
                break;
            default:
                break;
        }

        return sb.ToString();
    }
}
