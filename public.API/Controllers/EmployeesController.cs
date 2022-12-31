using MassTransit;
using Microservices.GitOps.MassTransit.Events;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Public.Api.Models;
using Public.Api.Services;

namespace Public.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IPublishEndpoint publishEndpoint;
    private readonly IBackendClient backendClient;
    private readonly IMongoCollection<Employee> employeesCollection;

    public EmployeesController(IPublishEndpoint publishEndpoint, IBackendClient backendClient, IMongoDatabase employeesDb)
    {
        this.publishEndpoint = publishEndpoint;
        this.backendClient = backendClient;
        this.employeesCollection = employeesDb.GetCollection<Employee>("employees");
    }

    [HttpGet]
    public async Task<IEnumerable<Employee>> GetEmployees() =>
        await backendClient.GetEmployeesAsync();


    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(Guid id)
    {
        var employee = await backendClient.GetEmployeeByIdAsync(id);

        if (employee is null)
            return NotFound();

        return employee;
    }

    [HttpPost]
    public async Task<ActionResult> CreateEmployee(EmployeeDto newEmployee)
    {
        // map to Employee model
        var employeeToAdd = new Employee { FirstName = newEmployee.FirstName, Prefix = newEmployee.Prefix, LastName = newEmployee.LastName };

        // Save
        await employeesCollection.InsertOneAsync(employeeToAdd);

        // Sending the message out to the topic for the rest of the system to consume
        await publishEndpoint.Publish(new EmployeeCreatedEvent { Employee = employeeToAdd });

        // Return Saved
        return Created("", employeeToAdd);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateEmployee(Guid id, EmployeeDto employeeUpdate)
    {
        // map to Employee model
        var updatedEmployee = new Employee
        {
            Id = id,
            FirstName = employeeUpdate.FirstName,
            Prefix = employeeUpdate.Prefix,
            LastName = employeeUpdate.LastName
        };

        // Update
        var result = await employeesCollection.ReplaceOneAsync(
            new FilterDefinitionBuilder<Employee>().Eq(e => e.Id, id),
            updatedEmployee
        );

        // Sending the message out to the topic for the rest of the system to consume
        await publishEndpoint.Publish(new EmployeeUpdatedEvent { Employee = updatedEmployee });

        // Return Saved
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmployee(Guid id)
    {
        await employeesCollection.DeleteOneAsync(e => e.Id == id);

        await publishEndpoint.Publish(new EmployeeDeletedEvent(id));

        return Ok();
    }
}
