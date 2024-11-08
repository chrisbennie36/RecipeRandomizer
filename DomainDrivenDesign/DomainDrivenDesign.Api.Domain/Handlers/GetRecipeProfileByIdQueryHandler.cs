using AutoMapper;
using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Data.Models;
using DomainDrivenDesign.Api.Domain.Queries;
using DomainDrivenDesign.Api.Domain.Results;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class GetRecipeProfileByIdQueryHandler: IRequestHandler<GetRecipeProfileByIdQuery, RecipeProfileResult?>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public GetRecipeProfileByIdQueryHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<RecipeProfileResult?> Handle(GetRecipeProfileByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            RecipeProfile? existingRecipeProfile = await appDbContext.RecipeProfiles.FindAsync(request.recipeProfileId);
            
            if(existingRecipeProfile == null)
            {
                return null;
            }

            return mapper.Map<RecipeProfileResult>(existingRecipeProfile);
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving a {nameof(RecipeProfile)} from the database: {e.Message}");
            return null;
        }
    }
}

