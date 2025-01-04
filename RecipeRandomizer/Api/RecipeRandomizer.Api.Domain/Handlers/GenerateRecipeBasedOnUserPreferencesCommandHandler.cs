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

public class GenerateRecipeBasedOnUserPreferencesCommandHandler : IRequestHandler<GenerateRecipeBasedOnUserPreferencesCommand, DomainResult<RecipeResult>>
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

    public async Task<DomainResult<RecipeResult>> Handle(GenerateRecipeBasedOnUserPreferencesCommand request, CancellationToken cancellationToken)
    {
        RecipeResult result = new RecipeResult();

        DomainResult<IEnumerable<RecipePreferenceModel>> userRecipePreferencesResult = await sender.Send(new GetUserRecipePreferencesQuery(request.userId));

        if(userRecipePreferencesResult.status != ResponseStatus.Success)
        {
            string errorMessage = $"Could not retrieve any user recipe preferences when generating a recipe URL";
            Log.Error(errorMessage);
            return new DomainResult<RecipeResult>(ResponseStatus.Error, null, errorMessage);
        }

        string recipeQueryParams = GenerateRecipeQuery(userRecipePreferencesResult.resultModel);

        if(string.IsNullOrEmpty(recipeQueryParams))
        {
            string errorMessage = "Could not generate query for recipe";
            Log.Error(errorMessage);
            return new DomainResult<RecipeResult>(ResponseStatus.Error, null, errorMessage);
        }
        
        GoogleCustomSearchResponse? customSearchResult = null;

        try
        {
            customSearchResult = await googleCustomSearchProxy.SearchAsync(new GoogleCustomSearchService.Api.Client.GoogleCustomSearchDto { QueryString = recipeQueryParams, PaginationToken = RandomizePaginationToken() });
        }
        catch(Exception e)
        {
            Log.Error($"Error calling the GoogleCustomSearchClient: {e.Message} {e.InnerException?.Message}");
            return new DomainResult<RecipeResult>(ResponseStatus.Error, null, "Error when calling the GoogleCustomSearchClient");
        }

        if(customSearchResult == null)
        {
            string message = "Response from the GoogleCustomSearchClient is null";
            Log.Warning(message);
            return new DomainResult<RecipeResult>(ResponseStatus.NotFound, null, message);
        }

        if(customSearchResult.ProblemDetails != null && customSearchResult.ProblemDetails.AdditionalProperties.ContainsKey("traceId"))
        {
            result.ErrorTraceId = (string)customSearchResult.ProblemDetails.AdditionalProperties["traceId"];
            Log.Error("Something went wrong when generating the recipe within the GoogleCustomSearchClient: Trace ID {traceId}", result.ErrorTraceId);
            return new DomainResult<RecipeResult>(ResponseStatus.Error, result, $"Something went wrong when generating the recipe within the GoogleCustomSearchClient: Trace ID {result.ErrorTraceId}");
        }

        IEnumerable<string> urls = FilterOutNonUsefulUrls(customSearchResult.Items.Select(i => i.Link).ToList());

        //Shouldn't happen, but if we receieve 10 results and they're all facebook/pinterest/reddit urls, then fetch a new set of results until there is at least one useful url
        while(!urls.Any())
        {
            customSearchResult = await googleCustomSearchProxy.SearchAsync(new GoogleCustomSearchService.Api.Client.GoogleCustomSearchDto { QueryString = recipeQueryParams, PaginationToken = RandomizePaginationToken() });

            if(customSearchResult == null)
            {
                return new DomainResult<RecipeResult>(ResponseStatus.NotFound, null);
            }

            urls = FilterOutNonUsefulUrls(customSearchResult.Items.Select(i => i.Link).ToList());
        }

        result.RecipeUrl = ChooseRandomResultUrl(urls.ToArray());
        return new DomainResult<RecipeResult>(ResponseStatus.Success, result);
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

        RecipePreferenceModel[] recipePreferencesArray = recipePreferences.Where(r => r.Type != RecipePreferenceType.General).ToArray();

        Random rand = new Random();

        RecipePreferenceType recipeTypeToSearchFor = recipePreferencesArray.First().Type;

        if(recipePreferencesArray.Count() > 1)
        {
            int randIndex = 0;
            try
            {
                randIndex = rand.Next(recipePreferences.Count() - 1);

                recipeTypeToSearchFor = recipePreferencesArray[randIndex].Type;
            }
            catch(IndexOutOfRangeException e)
            {
                Log.Error($"Index out of range when randomizing recipe type to search for, recipe type count: {recipePreferences.Count()}, index: { randIndex}");
            }
        }

        Log.Information($"Generating Recipe Query for Recipe Preference Type: {recipeTypeToSearchFor}");

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

        sb.Append($"{seafoodTypeToSearchFor.Name}+recipe");

        Log.Information($"Generated Seafood query: {sb.ToString()}");

        return sb.ToString();
    }

    private string GenerateMeatQuery(StringBuilder sb, RecipePreferenceModel[] meatPreferences)
    {
        Random rand = new Random();

        RecipePreferenceModel meatTypeToSearchFor = meatPreferences[rand.Next(meatPreferences.Count() - 1)];

        sb.Append($"{meatTypeToSearchFor.Name}+recipe");

        Log.Information($"Generated Meat query: {sb.ToString()}");

        return sb.ToString();
    }

    private string GenerateVegetarianQuery(StringBuilder sb, RecipePreferenceModel[] vegetarianPreferences)
    {
        Random rand = new Random();

        RecipePreferenceModel vegetarianTypeToSearchFor = vegetarianPreferences[rand.Next(vegetarianPreferences.Count() - 1)];

        sb.Append(string.Format($"vegetarian+{vegetarianTypeToSearchFor.Name}+recipe"));

        Log.Information($"Generated Vegetarian query: {sb.ToString()}");

        return sb.ToString();
    }
}
