using DomainDrivenDesign.Api.Data.Models;

namespace DomainDrivenDesign.Api.Data;

public class Message : EntityBase
{
    public string MessageText {get; set;} = string.Empty;
}
