using Backend.Api.Models;
using MassTransit;
using Microservices.GitOps.MassTransit.Events;
using MongoDB.Driver;

namespace Backend.Api.Events.Consumers;

public class EmployeeUpdatedEventConsumer : IConsumer<EmployeeUpdatedEvent>
{
    private readonly ILogger<EmployeeUpdatedEventConsumer> logger;
    private readonly IMongoCollection<Employee> employeesCollection;

    public EmployeeUpdatedEventConsumer(IMongoDatabase database, ILogger<EmployeeUpdatedEventConsumer> logger)
    {
        this.logger = logger;
        this.employeesCollection = database.GetCollection<Employee>("employees");
    }

    public async Task Consume(ConsumeContext<EmployeeUpdatedEvent> context)
    {
        this.logger.LogInformation("Employee updated event handler start");

        var result = await employeesCollection.ReplaceOneAsync(
            new FilterDefinitionBuilder<Employee>().Eq(e => e.Id, context.Message.Employee.Id),
            context.Message.Employee
        );

        this.logger.LogInformation("Employee with id: {id} update persisted", context.Message.Employee.Id);
    }
}