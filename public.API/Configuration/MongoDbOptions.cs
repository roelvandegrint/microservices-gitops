namespace Public.API.Configuration;

public record MongoDbOptions
{
    public string ConnectionString { get; set; } = null!;
    public string EmployeesDatabase { get; set; } = null!;
}