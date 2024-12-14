namespace RecipeRandomizer.Api.Domain.Proxies;

using GoogleCustomSearchService.Api.Client;
using Newtonsoft.Json;
using Serilog;
using GoogleClient = GoogleCustomSearchService.Api.Client.GoogleCustomSearchClient;

public class GoogleCustomSearchServiceProxy : IGoogleCustomSearchServiceProxy
{
    private readonly GoogleClient googleCustomSearchClient;
    public GoogleCustomSearchServiceProxy(GoogleClient googleCustomSearchClient)
    {
        this.googleCustomSearchClient = googleCustomSearchClient;
    }

    public async Task<GoogleCustomSearchResponse> SearchAsync(GoogleCustomSearchDto dto)
    {
        try
        {
            Log.Warning("About to call the GoogleCustomSearchClient");

            //ToDo: Determine why this isn't set during DI
            googleCustomSearchClient.BaseUrl = "http://localhost:5176";

            Log.Warning("Calling GoogleCustomSearchClient at URL: {url}", googleCustomSearchClient.BaseUrl);

            return await googleCustomSearchClient.GetResultsAsync(dto);
        }
        catch(ApiException e)
        {
            Log.Error($"Error when calling the GoogleCustomSearchClient: {e.Message} {e.InnerException?.Message}");

            ProblemDetails problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(e.Response);

            foreach(string key in problemDetails.AdditionalProperties.Keys)
            {
                if(key.ToLower() == "problemdetails")
                {
                    return new GoogleCustomSearchResponse 
                    {
                        ProblemDetails = JsonConvert.DeserializeObject<ProblemDetails>(problemDetails.AdditionalProperties[key].ToString())
                    };
                }
            }

            Log.Error("Could not deserialize problem details as expected");
            return new GoogleCustomSearchResponse();
        }
    }
}
