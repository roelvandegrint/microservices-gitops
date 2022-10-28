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
}
