using System.Text;
using AutoMapper;
using DomainDrivenDesign.Api.Domain.Commands;
using DomainDrivenDesign.Api.Domain.Models;
using DomainDrivenDesign.Api.Domain.Queries;
using DomainDrivenDesign.Api.Domain.Results;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class GenerateRecipeBasedOnRecipeProfileCommandHandler : IRequestHandler<GenerateRecipeBasedOnRecipeProfileCommand, string>
{
    private ISender sender;
    private IMapper mapper;

    public GenerateRecipeBasedOnRecipeProfileCommandHandler(ISender sender, IMapper mapper)
    {
        this.sender = sender;
        this.mapper = mapper;
    }

    public async Task<string> Handle(GenerateRecipeBasedOnRecipeProfileCommand request, CancellationToken cancellationToken)
    {
        RecipeProfileResult? recipeProfileResult = await sender.Send(new GetRecipeProfileByIdQuery(request.recipeProfileId));

        if(recipeProfileResult == null)
        {
            Log.Error($"Could not retrieve a {nameof(RecipeProfileResult)} when generating a recipe URL");
            return string.Empty;
        }

        return GenerateRecipeUrl(mapper.Map<RecipeProfileModel>(recipeProfileResult));
    }

    private string GenerateRecipeUrl(RecipeProfileModel recipeProfile)
    {
        StringBuilder sb = new StringBuilder("http://google.com/search?q=");

        switch(recipeProfile.MeatType)
        {
            case Shared.Enums.MeatType.Beef:
                sb.Append("beef+recipe");
                break;
            case Shared.Enums.MeatType.Pork:
                sb.Append("pork+recipe");
                break;
            case Shared.Enums.MeatType.Chicken:
                sb.Append("chicken+recipe");
                break;
            case Shared.Enums.MeatType.White:
                sb.Append("white+meat+recipe");
                break;
            case Shared.Enums.MeatType.Red:
                sb.Append("red+meat+recipe");
                break;
            default:
                break;
        }

        return sb.ToString();
    }
}
