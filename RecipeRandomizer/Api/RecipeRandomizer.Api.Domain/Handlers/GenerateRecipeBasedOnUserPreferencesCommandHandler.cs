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
using RecipeRandomizer.Api.Domain.Extensions;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GenerateRecipeBasedOnUserPreferencesCommandHandler : IRequestHandler<GenerateRecipeBasedOnUserPreferencesCommand, RecipeResult>
{
    //As per Google's API Page: The API will never return more than 100 results, even if more than 100 documents match the query, so setting the sum of start + num to a number greater than 100 
    //will produce an error. Our maximum is 90 as we are using the default value for num which is 10, so 10+90 = 100.
    const int MaxPaginationToken = 90;
    ReadOnlyCollection<string> domainsToRemove = new ReadOnlyCollection<string>(new List<string> {"facebook", "reddit", "pinterest", "forum" });

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

        IEnumerable<RecipePreferenceModel> userRecipePreferences = await sender.Send(new GetUserRecipePreferencesQuery(request.userId));

        if(!userRecipePreferences.Any())
        {
            Log.Error($"Could not retrieve any user recipe preferences when generating a recipe URL");
            return result;
        }

        string recipeQueryParams = GenerateRecipeQuery(userRecipePreferences);

        if(string.IsNullOrEmpty(recipeQueryParams))
        {
            Log.Error("Could not generate recipe query params for recipe");
            return result;
        }

        GoogleCustomSearchResponse? customSearchResult = await googleCustomSearchProxy.SearchAsync(new GoogleCustomSearchService.Api.Client.GoogleCustomSearchDto { QueryString = recipeQueryParams, PaginationToken = RandomizePaginationToken() });

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

    private string GenerateRecipeQuery(IEnumerable<RecipePreferenceModel> recipePreferences)
    {
        StringBuilder sb = new StringBuilder();

        RecipePreferenceModel[] recipePreferencesArray = recipePreferences.ToArray();

        Random rand = new Random();

        RecipePreferenceType recipeTypeToSearchFor = recipePreferencesArray[rand.Next(recipePreferences.Count() - 1)].Type;

        if(recipeTypeToSearchFor == RecipePreferenceType.Pescatarian)
        {
            return GenerateSeafoodQuery(sb, recipePreferences.GetSelectedUserPreferencesForType(RecipePreferenceType.Pescatarian).ToArray());
        }
        else if(recipeTypeToSearchFor == RecipePreferenceType.Meat)
        {
            return GenerateMeatQuery(sb, recipePreferences.GetSelectedUserPreferencesForType(RecipePreferenceType.Meat).ToArray());
        }
        else
        {
            return GenerateVegetarianQuery(sb, recipePreferences.GetSelectedUserPreferencesForType(RecipePreferenceType.Vegetarian).ToArray());
        }
    }

    private string GenerateSeafoodQuery(StringBuilder sb, RecipePreferenceModel[] seafoodPreferences)
    {
        Random rand = new Random();

        RecipePreferenceModel seafoodTypeToSearchFor = seafoodPreferences[rand.Next(seafoodPreferences.Count() - 1)];

        sb.Append($"{seafoodTypeToSearchFor}+recipe");

        return sb.ToString();
    }

    private string GenerateMeatQuery(StringBuilder sb, RecipePreferenceModel[] meatPreferences)
    {
        Random rand = new Random();

        RecipePreferenceModel meatTypeToSearchFor = meatPreferences[rand.Next(meatPreferences.Count() - 1)];

        sb.Append($"{meatTypeToSearchFor}+recipe");

        return sb.ToString();
    }

    private string GenerateVegetarianQuery(StringBuilder sb, RecipePreferenceModel[] vegetarianPreferences)
    {
        Random rand = new Random();

        RecipePreferenceModel vegetarianTypeToSearchFor = vegetarianPreferences[rand.Next(vegetarianPreferences.Count() - 1)];

        sb.Append(string.Format($"vegetarian+{vegetarianTypeToSearchFor}+recipe"));

        return sb.ToString();
    }
}
