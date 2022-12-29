using Backend.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

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
    public async Task<ActionResult<Employee>> GetEmployeeById(Guid id)
    {
        var employee = await dbContext.Employees.FindAsync(id);

        if (employee is null) return NotFound();

        return employee;
    }
}
