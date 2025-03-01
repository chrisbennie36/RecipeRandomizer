using MassTransit;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Infrastructure.Caching;
using RecipeRandomizer.Infrastructure.Repositories;
using RecipeRandomizer.Infrastructure.Repositories.Entities;
using Serilog;
using Utilities.RecipeRandomizer.Events;

namespace RecipeRandomizer.Api.Domain.EventConsumers;

public class RecipeRatedEventConsumer : IConsumer<RecipeRatedEvent>
{
    private readonly AppDbContext dbContext;
    private readonly ICacheService cacheService;

    public RecipeRatedEventConsumer(AppDbContext dbContext, ICacheService cacheService)
    {
        this.dbContext = dbContext;
        this.cacheService = cacheService;
    }

    public async Task Consume(ConsumeContext<RecipeRatedEvent> context)
    {        
        if(context.Message == null)
        {
            return;
        }

        RecipeRatedEvent recipeRatedEvent = context.Message;

        UserRecipeRating? existingRating = dbContext.UserRecipeRatings.SingleOrDefault(r => r.UserId == recipeRatedEvent.UserId && r.RecipeUrl == recipeRatedEvent.RecipeUrl);

        if (existingRating != null)
        {
            existingRating.Rating = recipeRatedEvent.RecipeRating;
            existingRating.UpdatedUtc = DateTime.UtcNow;
            dbContext.UserRecipeRatings.Update(existingRating);
        }
        else
        {
            UserRecipeRating recipeRatingEntity = new UserRecipeRating
            {
                UserId = context.Message.UserId,
                Rating = context.Message.RecipeRating,
                RecipeName = context.Message.RecipeName,
                RecipeUrl = context.Message.RecipeUrl,
                CreatedUtc = DateTime.UtcNow
            };

            dbContext.UserRecipeRatings.Add(recipeRatingEntity);
        }

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            Log.Error($"Error when saving Recipe Rating to the database: {e.Message} {e.InnerException?.Message}");
        }

        cacheService.RemoveData(CacheKeys.GetUserRecipeRatingsCacheKey(recipeRatedEvent.UserId));
    }
}
