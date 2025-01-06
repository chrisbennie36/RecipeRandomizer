using MassTransit;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Entities;
using Serilog;
using RecipeRatedEvent = Utilities.RecipeRandomizer.Events.RecipeRated;

namespace RecipeRandomizer.Api.Domain.EventConsumers;

public class RecipeRatedEventConsumer : IConsumer<RecipeRatedEvent>
{
    private readonly AppDbContext dbContext;

    public RecipeRatedEventConsumer(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<RecipeRatedEvent> context)
    {
        if(context.Message == null)
        {
            return;
        }

        RecipeRating recipeRatingEntity = new RecipeRating
        {
            UserId = context.Message.UserId,
            Rating = context.Message.RecipeRating,
            RecipeName = context.Message.RecipeName,
            RecipeUrl = context.Message.RecipeUrl
        };

        dbContext.RecipeRatings.Add(recipeRatingEntity);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Log.Error($"Error when saving Recipe Rating to the database: {e.Message} {e.InnerException?.Message}");
        }
    }
}
