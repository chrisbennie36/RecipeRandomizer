using System.Collections.ObjectModel;
using System.Text;
using AutoMapper;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Proxies;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Api.Domain.Results;
using RecipeRandomizer.Shared.Enums;
using GoogleCustomSearchService.Api.Client;
using MediatR;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GenerateRecipeBasedOnUserPreferencesCommandHandler : IRequestHandler<GenerateRecipeBasedOnUserPreferencesCommand, RecipeResult>
{
    const int MaxPaginationToken = 10000;
    ReadOnlyCollection<string> domainsToRemove = new ReadOnlyCollection<string>(new List<string> {"facebook", "reddit", "pinterest" });

    private ISender sender;
    private IMapper mapper;
    private IGoogleCustomSearchServiceProxy googleCustomSearchProxy;

    public GenerateRecipeBasedOnUserPreferencesCommandHandler(ISender sender, IMapper mapper, IGoogleCustomSearchServiceProxy googleCustomSearchProxy)
    {
        this.sender = sender;
        this.mapper = mapper;
        this.googleCustomSearchProxy = googleCustomSearchProxy;
    }

    public async Task<RecipeResult> Handle(GenerateRecipeBasedOnUserPreferencesCommand request, CancellationToken cancellationToken)
    {
        RecipeResult result = new RecipeResult();

        RecipeProfileResult? recipeProfileResult = await sender.Send(new GetRecipeProfileForUserQuery(request.userId));

        if(recipeProfileResult == null)
        {
            Log.Error($"Could not retrieve a {nameof(RecipeProfileResult)} when generating a recipe URL");
            return result;
        }

        string recipeQueryParams = GenerateRecipeQuery(mapper.Map<RecipeProfileModel>(recipeProfileResult));

        if(string.IsNullOrEmpty(recipeQueryParams))
        {
            Log.Error("Could not generate recipe query params for recipe");
            return result;
        }

        GoogleCustomSearchResponse? customSearchResult = await googleCustomSearchProxy.SearchAsync(new GoogleCustomSearchService.Api.Client.GoogleCustomSearchDto { QueryString = recipeQueryParams/*, PaginationToken = RandomizePaginationToken()*/ });

        if(customSearchResult == null)
        {
            return result;
        }

        IEnumerable<string> urls = FilterOutNonUsefulUrls(customSearchResult.Items.Select(i => i.Link).ToList());

        //Shouldn't happen, but if we receieve 10 results and they're all facebook/pinterest/reddit urls, then fetch a new set of results until there is at least one useful url
        while(!urls.Any())
        {
            customSearchResult = await googleCustomSearchProxy.SearchAsync(new GoogleCustomSearchService.Api.Client.GoogleCustomSearchDto { QueryString = recipeQueryParams, PaginationToken = RandomizePaginationToken() });

            if(customSearchResult == null)
            {
                return result;
            }

            urls = FilterOutNonUsefulUrls(customSearchResult.Items.Select(i => i.Link).ToList());
        }

        result.RecipeUrl = ChooseRandomResultUrl(urls.ToArray());
        return result;
    }

    //Should ensure that every time the user requests results from google, we minimise the chance of duplicate recipes coming back
    private int RandomizePaginationToken()
    {
        Random rand = new Random();

        return rand.Next(MaxPaginationToken);
    }

    private List<string> FilterOutNonUsefulUrls(List<string> urls)
    {
        foreach(string url in urls.ToList())
        {
            foreach(string toRemoveDomain in domainsToRemove)
            {
                if(url.ToLower().Contains(toRemoveDomain))
                {
                    urls.Remove(url);
                    break;
                }
            }
        }

        return urls;
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

        RecipePreferenceType[] recipePreferences = recipeProfile.RecipePreferences.ToArray();

        Random rand = new Random();

        RecipePreferenceType recipeTypeToSearchFor = recipePreferences[rand.Next(recipePreferences.Count())];

        if(recipeTypeToSearchFor == RecipePreferenceType.Pescatarian)
        {
            return GenerateSeafoodQuery(sb, recipeProfile.SeafoodPreferences.ToArray());
        }
        else if(recipeTypeToSearchFor == RecipePreferenceType.Meat)
        {
            return GenerateMeatQuery(sb, recipeProfile.MeatPreferences.ToArray());
        }
        else
        {
            return GenerateVegetarianQuery(sb, recipeProfile.VegetarianPreferences.ToArray());
        }
    }

    private string GenerateSeafoodQuery(StringBuilder sb, SeafoodPreferenceType[] seafoodPreferences)
    {
        Random rand = new Random();

        SeafoodPreferenceType seafoodTypeToSearchFor = seafoodPreferences[rand.Next(seafoodPreferences.Count())];

        sb.Append(string.Format("+with {seafoodTypeToSearchFor}", seafoodTypeToSearchFor.ToString()));

        return sb.ToString();
    }

    private string GenerateMeatQuery(StringBuilder sb, MeatPreferenceType[] meatPreferences)
    {
        Random rand = new Random();

        MeatPreferenceType meatTypeToSearchFor = meatPreferences[rand.Next(meatPreferences.Count())];

        sb.Append(string.Format("+with {meatTypeToSearchFor}", meatTypeToSearchFor.ToString()));

        return sb.ToString();
    }

    private string GenerateVegetarianQuery(StringBuilder sb, VegetarianPreferenceType[] vegetarianPreferences)
    {
        Random rand = new Random();

        VegetarianPreferenceType vegetarianTypeToSearchFor = vegetarianPreferences[rand.Next(vegetarianPreferences.Count())];

        sb.Append(string.Format("+with {vegetarianTypeToSearchFor}", vegetarianTypeToSearchFor.ToString()));

        return sb.ToString();
    }
}
