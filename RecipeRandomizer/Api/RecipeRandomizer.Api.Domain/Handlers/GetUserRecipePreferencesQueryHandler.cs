using RecipeRandomizer.Api.Domain.Queries;
using MediatR;
using Serilog;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Domain.Models;
using AutoMapper;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetUserRecipePreferencesQueryHandler : IRequestHandler<GetUserRecipePreferencesQuery, IEnumerable<RecipePreferenceModel>>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public GetUserRecipePreferencesQueryHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<RecipePreferenceModel>> Handle(GetUserRecipePreferencesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<UserRecipePreference> userRecipePreferences = await appDbContext.UserRecipePreferences.Where(ur => ur.UserId == request.userId)
                .Include(u => u.RecipePreferece).ToListAsync();

                return mapper.Map<List<RecipePreferenceModel>>(userRecipePreferences.Select(r => r.RecipePreferece));
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving recipe preferences from the database: {e.Message}");
            return Enumerable.Empty<RecipePreferenceModel>();
        }
    }
}
