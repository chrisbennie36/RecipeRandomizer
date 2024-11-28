namespace DomainDrivenDesign.Api.Domain.Proxies;

using GoogleCustomSearchService.Api.Client;

public interface IGoogleCustomSearchServiceProxy
{
    Task<GoogleCustomSearchResponse> SearchAsync(GoogleCustomSearchDto dto);
}
