using MediatR;
using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Commands;

public record AddSeafoodPreferencesForUserCommand(IEnumerable<SeafoodPreferenceType> preferences, int userId) : IRequest<bool>;
