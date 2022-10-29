using Backend.Api.Models;

namespace Microservices.GitOps.MassTransit.Events;

public class EmployeeCreatedEvent
{
    public Employee Employee { get; init; } = null!;
};
