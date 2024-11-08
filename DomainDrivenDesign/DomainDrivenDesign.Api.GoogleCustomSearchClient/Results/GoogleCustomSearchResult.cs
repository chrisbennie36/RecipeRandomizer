namespace DomainDrivenDesign.Api.GoogleCustomSearchClient.Results;

public class GoogleCustomSearchResult
{
    public IEnumerable<Item> Items { get; set; } = new List<Item>();
}

public class Item 
{
    public string Link { get; set; } = string.Empty;
}
