using Backend.Api.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Backend.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IMongoDatabase database;

    public EmployeesController(IMongoDatabase database)
    {
        this.database = database;
    }

    [HttpGet]
    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        var collection = database.GetCollection<Employee>("employees");

        return await collection.Find(x => true).ToListAsync();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(Guid id)
    {
        var collection = database.GetCollection<Employee>("employees");

        var employee = await collection.Find(x => x.Id == id).SingleOrDefaultAsync();

        if (employee is null)
            return NotFound();

        return employee;
    }
}
