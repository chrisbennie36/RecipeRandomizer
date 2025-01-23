using HotChocolate;
using RecipeRandomizer.Api.Data.Entities;

namespace RecipeRandomizer.Api.Data.GraphQlQueryProviders;

public class RecipeRatingQueryProvider
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<UserRecipeRating> GetRecipeRatings([Service] AppDbContext dbContext) => dbContext.UserRecipeRatings;
}
