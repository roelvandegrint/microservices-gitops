using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Public.Api.Models;

namespace Public.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly EmployeeDbContext dbContext;

    public EmployeesController(EmployeeDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public IEnumerable<Employee> GetEmployees() => dbContext.Employees;

    [HttpGet("{id}")]
    public ActionResult<Employee> GetEmployeeById(int id)
    {
        var employee = dbContext.Employees.Find(id);
        if(employee is null) return NotFound();
        return employee;
    }

    [HttpPost]
    public async Task<ActionResult> CreateEmployee(EmployeeDto newEmployee)
    {
        // map to Employee model
        var employeeToAdd = new Employee { FirstName = newEmployee.FirstName, Prefix = newEmployee.Prefix, LastName = newEmployee.LastName };

        // Save
        await dbContext.Employees.AddAsync(employeeToAdd);
        await dbContext.SaveChangesAsync();

        // Return Saved
        return Created("", employeeToAdd);
    }
}

public record EmployeeDto
{
    [Required]
    public string FirstName { get; init; } = null!;
    public string? Prefix { get; init; }
    [Required]
    public string LastName { get; init; } = null!;
}