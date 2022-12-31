namespace Backend.Api.Configuration;

public class MassTransitOptions
{
    public string AzureServiceBusConnectionString { get; set; } = null!;
    public string EmployeeCreatedSubscriptionName { get; set; } = null!;
    public string EmployeeUpdatedSubscriptionName { get; set; } = null!;
    public string EmployeeDeletedSubscriptionName { get; set; } = null!;
}