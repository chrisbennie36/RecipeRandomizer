using AutoMapper;
using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Data.Models;
using DomainDrivenDesign.Api.Domain.Queries;
using DomainDrivenDesign.Api.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class GetRecipeProfileForUserQueryHandler : IRequestHandler<GetRecipeProfileForUserQuery, RecipeProfileResult?>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;
    private readonly ILogger<GetRecipeProfileForUserQueryHandler> logger;

    public GetRecipeProfileForUserQueryHandler(AppDbContext appDbContext, IMapper mapper, ILogger<GetRecipeProfileForUserQueryHandler> logger)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<RecipeProfileResult?> Handle(GetRecipeProfileForUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            RecipeProfile? recipeProfileForUser = await appDbContext.RecipeProfiles.FirstOrDefaultAsync(r => r.UserId == request.userId);
            
            if(recipeProfileForUser == null)
            {
                return null;
            }

            return mapper.Map<RecipeProfileResult>(recipeProfileForUser);
        }
        catch(Exception e)
        {
            logger.LogError($"Error when retrieving a {nameof(RecipeProfile)} from the database: {e.Message}");
            return null;
        }
    }
}
