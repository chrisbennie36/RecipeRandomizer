using RecipeRandomizer.Api.Domain.Clients.Dtos;
using RecipeRandomizer.Api.Domain.Clients.Responses;
using Refit;

namespace RecipeRandomizer.Api.Domain.Clients;

public interface IGoogleCustomSearchClient
{
    [Post("/api/GoogleCustomSearch")]
    Task<GoogleCustomSearchResponse> SearchAsync(GoogleCustomSearchDto dto);
}
