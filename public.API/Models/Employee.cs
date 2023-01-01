namespace Public.Api.Models;

public class Employee
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FirstName { get; init; } = null!;
    public string? Prefix { get; init; }
    public string LastName { get; init; } = null!;
}