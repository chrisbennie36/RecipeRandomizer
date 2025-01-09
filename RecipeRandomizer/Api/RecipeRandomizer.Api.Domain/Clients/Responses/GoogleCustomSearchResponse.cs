namespace RecipeRandomizer.Api.Domain.Clients.Responses;

public class GoogleCustomSearchResponse
{
    public IEnumerable<Item> Items { get; set; } = new List<Item>();
}

public class Item 
{
    public string Link { get; set; } = string.Empty; 
}
