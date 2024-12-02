using MediatR;
using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Commands;

public record AddMeatPreferencesForUserCommand(IEnumerable<MeatPreferenceType> meatPreferences, int userId) : IRequest<bool>;
