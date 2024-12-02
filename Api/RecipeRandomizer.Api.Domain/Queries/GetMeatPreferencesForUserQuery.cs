using MediatR;
using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetMeatPreferencesForUserQuery(int userId) : IRequest<IEnumerable<MeatPreferenceType>>;
