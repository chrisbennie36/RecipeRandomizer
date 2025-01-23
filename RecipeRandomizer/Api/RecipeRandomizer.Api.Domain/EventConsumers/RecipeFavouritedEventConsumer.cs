using MassTransit;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Entities;
using Serilog;
using Utilities.RecipeRandomizer.Events;

namespace RecipeRandomizer.Api.Domain.EventConsumers;

public class RecipeFavouritedEventConsumer : IConsumer<RecipeFavouritedEvent>
{
    private readonly AppDbContext dbContext;

    public RecipeFavouritedEventConsumer(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<RecipeFavouritedEvent> context)
    {        
        if(context.Message == null)
        {
            return;
        }

        RecipeFavouritedEvent recipeFavouritedEvent = context.Message;

        UserRecipeFavourite? existingFavourite = dbContext.UserRecipeFavourites.SingleOrDefault(r => r.UserId == recipeFavouritedEvent.UserId && r.RecipeUrl == recipeFavouritedEvent.RecipeUrl.ToString());

        if (existingFavourite == null)
        {
            return;
        }

        UserRecipeFavourite userRecipeFavourite = new UserRecipeFavourite
        {
            UserId = context.Message.UserId,
            RecipeName = context.Message.RecipeName,
            RecipeUrl = context.Message.RecipeUrl.ToString()
        };

        dbContext.UserRecipeFavourites.Add(userRecipeFavourite);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            Log.Error($"Error when saving {nameof(UserRecipeFavourite)} to the database: {e.Message} {e.InnerException?.Message}");
        }
    }
}
