using System.ComponentModel.DataAnnotations;

namespace Public.Api.Models;

public record EmployeeDto
{
    [Required]
    public string FirstName { get; init; } = null!;
    public string? Prefix { get; init; }
    [Required]
    public string LastName { get; init; } = null!;
}