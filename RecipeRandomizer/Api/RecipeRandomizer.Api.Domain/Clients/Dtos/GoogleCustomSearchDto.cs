namespace RecipeRandomizer.Api.Domain.Clients.Dtos;

public class GoogleCustomSearchDto
{
    public string QueryString { get; set; }= string.Empty;
    public int PaginationToken { get; set; }
}
