using Public.Api.Models;

namespace Microservices.GitOps.MassTransit.Events;

public class EmployeeUpdatedEvent
{
    public Employee Employee { get; init; } = null!;
};
