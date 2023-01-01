using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Public.Api.Models;
using Public.API.Persistence;

public class EmployeeDbContext : DbContext
{
    private readonly CosmosDbOptions cosmosDbOptions;

    public EmployeeDbContext(IOptions<CosmosDbOptions> cosmosDbOptions)
    {
        this.cosmosDbOptions = cosmosDbOptions.Value;
    }

    public DbSet<Employee> Employees { get; set; } = null!;

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseCosmos(
            cosmosDbOptions.AccountEndpoint,
            cosmosDbOptions.AccountKey,
            cosmosDbOptions.DatabaseName
        );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultContainer("employees");
        modelBuilder.Entity<Employee>().HasPartitionKey(e => e.Id);
    }
}