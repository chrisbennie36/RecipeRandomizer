namespace RecipeRandomizer.Api.WebApplication.Extensions;

using RecipeRandomizer.Api.Domain.Results;
using Microsoft.AspNetCore.Mvc;

public static class DomainResultExtensions
{
    public static ActionResult ToActionResult(this DomainResult domainResult)
    {
        return MapDomainResult(domainResult);
    }

    public static ActionResult ToActionResult<T>(this DomainResult<T> domainResult)
    {
        return MapResponseModelDomainResult<T>(domainResult);
    }

    private static ActionResult MapDomainResult(DomainResult domainResult)
    {
        switch(domainResult.status)
        {
            case ResponseStatus.Success:
                return new OkResult();
            case ResponseStatus.NotFound:
                return new NotFoundResult();
            default:
                return new BadRequestObjectResult(domainResult.errorMessage);
        }
    }

    private static ActionResult MapResponseModelDomainResult<T>(DomainResult<T> domainResult)
    {
        switch(domainResult.status)
        {
            case ResponseStatus.Success:
                return new OkObjectResult(domainResult.resultModel);
            case ResponseStatus.NotFound:
                return new NotFoundResult();
            default:
                return new BadRequestObjectResult(domainResult.errorMessage);
        }
    }
}
