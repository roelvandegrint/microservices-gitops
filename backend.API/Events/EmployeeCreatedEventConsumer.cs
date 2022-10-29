using MassTransit;
using Microservices.GitOps.MassTransit.Events;

namespace Backend.Api.Events.Consumers;

public class EmployeeCreatedEventConsumer : IConsumer<EmployeeCreatedEvent>
{
    private readonly EmployeeDbContext dbContext;
    private readonly ILogger<EmployeeCreatedEventConsumer> logger;

    public EmployeeCreatedEventConsumer(EmployeeDbContext dbContext, ILogger<EmployeeCreatedEventConsumer> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
    {
        this.logger.LogInformation("Employee created event handler start");

        dbContext.Employees.Add(context.Message.Employee);
        await dbContext.SaveChangesAsync();

        this.logger.LogInformation("New employee persisted in database");
        this.logger.LogInformation("Employee created event handler finished");       
    }
}