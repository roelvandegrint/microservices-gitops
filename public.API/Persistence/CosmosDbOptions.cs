namespace Public.API.Persistence;

public record CosmosDbOptions
{
    public string AccountEndpoint { get; init; } = null!;
    public string AccountKey { get; init; } = null!;
    public string DatabaseName { get; init; } = null!;
}