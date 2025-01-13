using RecipeRandomizer.Api.Domain.Queries;
using MediatR;
using RecipeRandomizer.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Domain.Models;
using AutoMapper;
using Utilities.ResultPattern;
using RecipeRandomizer.Api.Data.Repositories;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetUserRecipePreferencesQueryHandler : IRequestHandler<GetUserRecipePreferencesQuery, DomainResult<IEnumerable<RecipePreferenceModel>>>
{
    private readonly UserRecipePreferencesRepository userRecipePreferencesRepository;
    private readonly IMapper mapper;

    public GetUserRecipePreferencesQueryHandler(UserRecipePreferencesRepository userRecipePreferencesRepository, IMapper mapper)
    {
        this.userRecipePreferencesRepository = userRecipePreferencesRepository;
        this.mapper = mapper;
    }

    public async Task<DomainResult<IEnumerable<RecipePreferenceModel>>> Handle(GetUserRecipePreferencesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<UserRecipePreference> userRecipePreferences = await userRecipePreferencesRepository.GetUserRecipePreferences(request.userId);

            List<RecipePreferenceModel> mappedRecipePreferences = mapper.Map<List<RecipePreferenceModel>>(userRecipePreferences.Select(r => r.RecipePreferece));
            return new DomainResult<IEnumerable<RecipePreferenceModel>>(ResponseStatus.Success, mappedRecipePreferences);
        }
        catch(DbUpdateException)
        {
            return new DomainResult<IEnumerable<RecipePreferenceModel>>(ResponseStatus.Error, Enumerable.Empty<RecipePreferenceModel>(), $"Unable to retrieve {nameof(UserRecipePreference)}(s) from the database");
        }
    }
}
