using MediatR;
using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Commands;

public record AddVegetarianPreferencesForUserCommand(IEnumerable<VegetarianPreferenceType> vegetarianPreferences, int userId) : IRequest<bool>;
