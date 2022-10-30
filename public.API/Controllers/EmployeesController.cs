using MassTransit;
using Microservices.GitOps.MassTransit.Events;
using Microsoft.AspNetCore.Mvc;
using Public.Api.Models;

namespace Public.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly EmployeeDbContext dbContext;
    private readonly IPublishEndpoint publishEndpoint;

    public EmployeesController(EmployeeDbContext dbContext, IPublishEndpoint publishEndpoint)
    {
        this.dbContext = dbContext;
        this.publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public IEnumerable<Employee> GetEmployees() => dbContext.Employees;

    [HttpGet("{id}")]
    public ActionResult<Employee> GetEmployeeById(int id)
    {
        var employee = dbContext.Employees.Find(id);
        if (employee is null) return NotFound();
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

        // Sending the message out to the topic for the rest of the system to consume
        await publishEndpoint.Publish(new EmployeeCreatedEvent { Employee = employeeToAdd });

        // Return Saved
        return Created("", employeeToAdd);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmployee(Guid id)
    {
        var employeeToDelete = dbContext.Employees.Find(id);
        if (employeeToDelete is null) return NotFound();

        dbContext.Employees.Remove(employeeToDelete);
        await dbContext.SaveChangesAsync();

        await publishEndpoint.Publish(new EmployeeDeletedEvent(id));

        return Ok();
    }
}
