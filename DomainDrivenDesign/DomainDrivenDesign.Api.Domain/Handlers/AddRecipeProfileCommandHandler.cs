using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Data.Models;
using DomainDrivenDesign.Api.Domain.Commands;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class AddRecipeProfileCommandHandler: IRequestHandler<AddRecipeProfileCommand, bool>
{
    private readonly AppDbContext appDbContext;

    public AddRecipeProfileCommandHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<bool> Handle(AddRecipeProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            appDbContext.RecipeProfiles.Add(new RecipeProfile 
            {
                UserId = request.userId,
                MeatType = request.recipeProfile.MeatType,
                DietType = request.recipeProfile.DietType,
                CreatedUtc = DateTime.UtcNow
            });

            await appDbContext.SaveChangesAsync();
        }
        catch(Exception e)
        {
            Log.Error($"Error when writing a {nameof(RecipeProfile)} to the database: {e.Message}");
            return false;
        }

        return true;
    }
}

