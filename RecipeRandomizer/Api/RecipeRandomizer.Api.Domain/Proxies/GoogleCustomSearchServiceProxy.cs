namespace RecipeRandomizer.Api.Domain.Proxies;

using GoogleCustomSearchService.Api.Client;
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
        //ToDo: Determine why this isn't set during DI
        googleCustomSearchClient.BaseUrl = "http://projects.eba-mwswa2uy.us-east-1.elasticbeanstalk.com";
        return await googleCustomSearchClient.GetResultsAsync(dto);
    }
}
