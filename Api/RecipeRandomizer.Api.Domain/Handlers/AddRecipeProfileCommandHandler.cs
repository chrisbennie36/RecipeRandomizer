using AutoMapper;
using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Data.Models;
using DomainDrivenDesign.Api.Domain.Commands;
using DomainDrivenDesign.Api.Domain.Results;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class AddRecipeProfileCommandHandler: IRequestHandler<AddRecipeProfileCommand, RecipeProfileResult?>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public AddRecipeProfileCommandHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<RecipeProfileResult?> Handle(AddRecipeProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipeProfile = appDbContext.RecipeProfiles.Add(new RecipeProfile 
            {
                UserId = request.userId,
                MeatType = request.recipeProfile.MeatType,
                DietType = request.recipeProfile.DietType,
                CreatedUtc = DateTime.UtcNow
            });

            await appDbContext.SaveChangesAsync();

            return mapper.Map<RecipeProfileResult>(recipeProfile.Entity);
        }
        catch(Exception e)
        {
            Log.Error($"Error when writing a {nameof(RecipeProfile)} to the database: {e.Message}");
            return null;
        }
    }
}

