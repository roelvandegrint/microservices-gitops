using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Backend.API.Persistence;
using Backend.Api.Models;

public class EmployeeDbContext : DbContext
{
    private readonly string connectionString;

    public EmployeeDbContext(IOptions<DatabaseOptions> databaseOptions, ILogger<EmployeeDbContext> logger)
    {
        logger.LogInformation($"Connectionstring: {databaseOptions.Value.ConnectionString}");
        connectionString = databaseOptions.Value.ConnectionString;
    }

    public DbSet<Employee> Employees { get; set; } = null!;

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}