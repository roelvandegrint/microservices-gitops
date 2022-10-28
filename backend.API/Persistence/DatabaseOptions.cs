namespace Backend.API.Persistence;

public record DatabaseOptions
{
    public string ConnectionString { get; init; } = null!;
}