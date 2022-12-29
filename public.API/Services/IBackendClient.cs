using Public.Api.Models;

namespace Public.Api.Services;

public interface IBackendClient
{
    Task<IEnumerable<Employee>> GetEmployeesAsync();

    Task<Employee?> GetEmployeeByIdAsync(Guid id);
}
