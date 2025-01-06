using MassTransit;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Entities;
using Serilog;
using RecipeRatedEvent = Utilities.RecipeRandomizer.Events.RecipeRated;

namespace RecipeRandomizer.Events.RecipeRated;

public class RecipeRatedConsumer : IConsumer<RecipeRatedEvent>
{
    private readonly AppDbContext dbContext;

    public RecipeRatedConsumer(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<RecipeRatedEvent> context)
    {
        if(context.Message == null)
        {
            return;
        }

        RecipeRating recipeRating = new RecipeRating 
        {
            UserId = context.Message.UserId,
            RecipeName = context.Message.RecipeName,
            RecipeUrl = context.Message.RecipeUrl,
            Rating = context.Message.RecipeRating
        };

        dbContext.RecipeRatings.Add(recipeRating);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch(Exception e)
        {
            Log.Error($"An error occurred when trying to save a Recipe Rating to the database, error: {e.Message} {e.InnerException?.Message}");
        }
    }
}
