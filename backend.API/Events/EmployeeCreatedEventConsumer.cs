using Backend.Api.Models;
using MassTransit;
using Microservices.GitOps.MassTransit.Events;
using MongoDB.Driver;

namespace Backend.Api.Events.Consumers;

public class EmployeeCreatedEventConsumer : IConsumer<EmployeeCreatedEvent>
{
    private readonly ILogger<EmployeeCreatedEventConsumer> logger;
    private readonly IMongoCollection<Employee> employeesCollection;

    public EmployeeCreatedEventConsumer(IMongoDatabase database, ILogger<EmployeeCreatedEventConsumer> logger)
    {
        this.logger = logger;
        this.employeesCollection = database.GetCollection<Employee>("employees");
    }

    public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
    {
        this.logger.LogInformation("Employee created event handler start");

        await employeesCollection.InsertOneAsync(context.Message.Employee);

        this.logger.LogInformation("Employee with id: {id} persisted", context.Message.Employee.Id);

    }
}