using DomainDrivenDesign.Api.Domain.Models;
using DomainDrivenDesign.Shared.Enums;
using MediatR;

namespace DomainDrivenDesign.Api.Domain.Commands
{
    public record AddUserCommand(string username, string password, string emailAddress, UserRole role, RecipeProfileModel? recipeProfileModel) : IRequest<bool>;
}