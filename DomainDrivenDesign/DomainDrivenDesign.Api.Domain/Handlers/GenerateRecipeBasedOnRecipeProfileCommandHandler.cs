using System.Text;
using AutoMapper;
using DomainDrivenDesign.Api.Domain.Commands;
using DomainDrivenDesign.Api.Domain.Models;
using DomainDrivenDesign.Api.Domain.Proxies;
using DomainDrivenDesign.Api.Domain.Queries;
using DomainDrivenDesign.Api.Domain.Results;
using DomainDrivenDesign.Shared.Enums;
using GoogleCustomSearchService.Api.Client;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class GenerateRecipeBasedOnRecipeProfileCommandHandler : IRequestHandler<GenerateRecipeBasedOnRecipeProfileCommand, RecipeResult>
{
    private ISender sender;
    private IMapper mapper;
    private IGoogleCustomSearchServiceProxy googleCustomSearchProxy;

    public GenerateRecipeBasedOnRecipeProfileCommandHandler(ISender sender, IMapper mapper, IGoogleCustomSearchServiceProxy googleCustomSearchProxy)
    {
        this.sender = sender;
        this.mapper = mapper;
        this.googleCustomSearchProxy = googleCustomSearchProxy;
    }

    public async Task<RecipeResult> Handle(GenerateRecipeBasedOnRecipeProfileCommand request, CancellationToken cancellationToken)
    {
        RecipeResult result = new RecipeResult();

        RecipeProfileResult? recipeProfileResult = await sender.Send(new GetRecipeProfileByIdQuery(request.recipeProfileId));

        if(recipeProfileResult == null)
        {
            Log.Error($"Could not retrieve a {nameof(RecipeProfileResult)} when generating a recipe URL");
            return result;
        }

        string recipeQueryParams = GenerateRecipeQuery(mapper.Map<RecipeProfileModel>(recipeProfileResult));

        if(string.IsNullOrEmpty(recipeQueryParams))
        {
            Log.Error($"Could not generate recipe query params for {typeof(MeatType)}: {recipeProfileResult.MeatType}");
            return result;
        }

        GoogleCustomSearchResponse? customSearchResult = await googleCustomSearchProxy.SearchAsync(new GoogleCustomSearchService.Api.Client.GoogleCustomSearchDto { QueryString = recipeQueryParams });

        if(customSearchResult == null)
        {
            return result;
        }

        IEnumerable<string> urls = customSearchResult.Items.Select(i => i.Link);

        result.RecipeUrl = ChooseRandomResultUrl(urls.ToArray());
        return result;
    }

    private string ChooseRandomResultUrl(string[] urls)
    {
        Random rand = new Random();
        int index = rand.Next(urls.Length);

        return urls[index];
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
