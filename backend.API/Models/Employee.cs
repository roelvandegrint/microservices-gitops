namespace Backend.Api.Models;

public class Employee
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string? Prefix { get; set; }
    public string LastName { get; set; } = null!;
}
