using Backend.Api.Models;
using MassTransit;
using Microservices.GitOps.MassTransit.Events;
using MongoDB.Driver;

namespace Backend.Api.Events.Consumers;

public class EmployeeDeletedEventConsumer : IConsumer<EmployeeDeletedEvent>
{
    private readonly ILogger<EmployeeDeletedEventConsumer> logger;
    private readonly IMongoCollection<Employee> employeesCollection;

    public EmployeeDeletedEventConsumer(ILogger<EmployeeDeletedEventConsumer> logger, IMongoDatabase employeesDb)
    {
        this.logger = logger;
        this.employeesCollection = employeesDb.GetCollection<Employee>("employees");
    }

    public async Task Consume(ConsumeContext<EmployeeDeletedEvent> context)
    {
        this.logger.LogInformation("Employee deleted event handler start");

        var result = await employeesCollection.DeleteOneAsync(
            new FilterDefinitionBuilder<Employee>().Eq(e => e.Id, context.Message.EmployeeId)
        );
        
        if (result.DeletedCount == 0)
        {
            logger.LogInformation("Employee with ID {id} not found, skipping delete", context.Message.EmployeeId);
            return;
        }

        this.logger.LogInformation("Employee with id {id} deleted", context.Message.EmployeeId);
    }
}