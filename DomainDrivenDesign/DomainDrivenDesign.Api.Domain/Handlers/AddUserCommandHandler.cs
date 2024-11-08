using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Domain.Commands;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class AddUserCommandHandler: IRequestHandler<AddUserCommand, bool>
{
    private readonly AppDbContext appDbContext;
    private readonly ISender sender;

    public AddUserCommandHandler(AppDbContext appDbContext, ISender sender)
    {
        this.appDbContext = appDbContext;
        this.sender = sender;
    }

    public async Task<bool> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = appDbContext.Users.Add(new User 
            {
                Username = request.username,
                Password = request.password,
                EmailAddress = request.emailAddress,
                Role = request.role,
                CreatedUtc = DateTime.UtcNow
            });

            await appDbContext.SaveChangesAsync();

            if(request.recipeProfileModel != null)
            {
                await sender.Send(new AddRecipeProfileCommand(request.recipeProfileModel, user.Entity.Id));
            }
        }
        catch(Exception e)
        {
            Log.Error($"Error when writing a {nameof(User)} to the database: {e.Message}");
            return false;
        }

        return true;
    }
}

