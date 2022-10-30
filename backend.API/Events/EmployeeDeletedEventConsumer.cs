using MassTransit;
using Microservices.GitOps.MassTransit.Events;

namespace Backend.Api.Events.Consumers;

public class EmployeeDeletedEventConsumer : IConsumer<EmployeeDeletedEvent>
{
    private readonly EmployeeDbContext dbContext;
    private readonly ILogger<EmployeeDeletedEventConsumer> logger;

    public EmployeeDeletedEventConsumer(EmployeeDbContext dbContext, ILogger<EmployeeDeletedEventConsumer> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task Consume(ConsumeContext<EmployeeDeletedEvent> context)
    {
        this.logger.LogInformation("Employee deleted event handler start");

        var employeeToDelete = await dbContext.Employees.FindAsync(context.Message.EmployeeId);
        if(employeeToDelete is null)
        {
            logger.LogInformation("Employee with ID {id} not found, skipping delete", context.Message.EmployeeId);
            return;
        }

        dbContext.Employees.Remove(employeeToDelete);
        await dbContext.SaveChangesAsync();
        this.logger.LogInformation("Employee with id {id} deleted", context.Message.EmployeeId);
    }
}