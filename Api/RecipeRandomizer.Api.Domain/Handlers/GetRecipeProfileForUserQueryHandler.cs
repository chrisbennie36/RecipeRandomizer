using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Api.Domain.Results;
using MediatR;
using RecipeRandomizer.Shared.Enums;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetRecipeProfileForUserQueryHandler : IRequestHandler<GetRecipeProfileForUserQuery, RecipeProfileResult?>
{
    private readonly ISender sender;

    public GetRecipeProfileForUserQueryHandler(ISender sender)
    {
        this.sender = sender;
    }

    public async Task<RecipeProfileResult?> Handle(GetRecipeProfileForUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<RecipePreferenceType> recipePreferences = await sender.Send(new GetRecipePreferencesForUserQuery(request.userId));
            IEnumerable<SeafoodPreferenceType> seafoodPreferences = await sender.Send(new GetSeafoodPreferencesForUserQuery(request.userId));
            IEnumerable<MeatPreferenceType> meatPreferences = await sender.Send(new GetMeatPreferencesForUserQuery(request.userId));
            IEnumerable<VegetarianPreferenceType> vegetarianPreferences = await sender.Send(new GetVegetarianPreferencesForUserQuery(request.userId));

            return new RecipeProfileResult
            {
                RecipePreferences = recipePreferences,
                SeafoodPreferences = seafoodPreferences,
                MeatPreferences = meatPreferences,
                VegetarianPreferences = vegetarianPreferences
            };
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving recipe preferences from the database: {e.Message}");
            return null;
        }
    }
}
