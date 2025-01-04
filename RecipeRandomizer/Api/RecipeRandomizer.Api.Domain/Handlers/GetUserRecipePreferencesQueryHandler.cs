using RecipeRandomizer.Api.Domain.Queries;
using MediatR;
using Serilog;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Results;
using AutoMapper;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetUserRecipePreferencesQueryHandler : IRequestHandler<GetUserRecipePreferencesQuery, DomainResult<IEnumerable<RecipePreferenceModel>>>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public GetUserRecipePreferencesQueryHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<DomainResult<IEnumerable<RecipePreferenceModel>>> Handle(GetUserRecipePreferencesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<UserRecipePreference> userRecipePreferences = await appDbContext.UserRecipePreferences
            .AsNoTracking()
            .Where(ur => ur.UserId == request.userId)
                .Include(u => u.RecipePreferece).ToListAsync();

            List<RecipePreferenceModel> mappedRecipePreferences = mapper.Map<List<RecipePreferenceModel>>(userRecipePreferences.Select(r => r.RecipePreferece));
            return new DomainResult<IEnumerable<RecipePreferenceModel>>(ResponseStatus.Success, mappedRecipePreferences);
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving Recipe Preferences from the database: {e.Message} {e.InnerException?.Message}");
            return new DomainResult<IEnumerable<RecipePreferenceModel>>(ResponseStatus.Error, Enumerable.Empty<RecipePreferenceModel>(), "Error when retrieving Recipe Preferences from the database");
        }
    }
}
