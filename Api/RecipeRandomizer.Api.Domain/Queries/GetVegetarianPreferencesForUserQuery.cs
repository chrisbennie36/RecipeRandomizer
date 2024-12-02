using MediatR;
using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetVegetarianPreferencesForUserQuery(int userId) : IRequest<IEnumerable<VegetarianPreferenceType>>;
