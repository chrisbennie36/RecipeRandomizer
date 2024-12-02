using MediatR;
using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetSeafoodPreferencesForUserQuery(int userId) : IRequest<IEnumerable<SeafoodPreferenceType>>;
