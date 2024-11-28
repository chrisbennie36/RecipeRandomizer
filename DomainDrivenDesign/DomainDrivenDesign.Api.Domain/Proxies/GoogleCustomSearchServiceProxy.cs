namespace DomainDrivenDesign.Api.Domain.Proxies;

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
        return await googleCustomSearchClient.GetResultsAsync(dto);
    }
}
