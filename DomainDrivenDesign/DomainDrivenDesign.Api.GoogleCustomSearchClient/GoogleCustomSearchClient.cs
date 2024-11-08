using DomainDrivenDesign.Api.GoogleCustomSearchClient.Interfaces;
using DomainDrivenDesign.Api.GoogleCustomSearchClient.Results;
using Newtonsoft.Json;
using RestSharp;
using Serilog;

namespace DomainDrivenDesign.Api.GoogleCustomSearchClient;

public class GoogleCustomSearchClient : IGoogleCustomSearchClient
{
    private readonly IRestClient client;

    public GoogleCustomSearchClient()
    {
        client = new RestClient();
    }

    public async Task<GoogleCustomSearchResult?> GetResults(string queryParams)
    {
        RestRequest request = new RestRequest($"https://www.googleapis.com/customsearch/v1?key=AIzaSyBPg_RNOYaXH3Qwz1RAsWUlp-h4rAoY9pM&cx=921e5e54836d64b7f&{queryParams}");

        var response = await client.GetAsync(request);

        if(response == null || string.IsNullOrWhiteSpace(response.Content))
        {
            return null;
        }

        if(!response.IsSuccessful)
        {
            Log.Error($"Error when calling the Google Custom Search API, error code: {response.StatusCode}, error message: {response.ErrorMessage}, error exception: {response.ErrorException}");
            return null;
        }

        try
        {
            GoogleCustomSearchResult? result = JsonConvert.DeserializeObject<GoogleCustomSearchResult>(response.Content);
            return result;
        }
        catch(Exception e)
        {
            Log.Error($"Error when deserializating the Google Custom Search results, exception message: {e.Message}");
            return null;
        }
    }

}
