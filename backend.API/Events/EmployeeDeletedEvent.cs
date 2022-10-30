namespace Microservices.GitOps.MassTransit.Events;

public class EmployeeDeletedEvent
{
    public Guid EmployeeId { get; init; }
}
