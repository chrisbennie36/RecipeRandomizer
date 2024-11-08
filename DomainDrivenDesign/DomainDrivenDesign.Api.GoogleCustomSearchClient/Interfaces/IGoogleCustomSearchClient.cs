using DomainDrivenDesign.Api.GoogleCustomSearchClient.Results;

namespace DomainDrivenDesign.Api.GoogleCustomSearchClient.Interfaces;

public interface IGoogleCustomSearchClient
{
    Task<GoogleCustomSearchResult?> GetResults(string queryParams);
}
