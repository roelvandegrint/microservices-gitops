using Public.Api.Models;

namespace Public.Api.Services;

public class BackendClient : IBackendClient
{
    private readonly HttpClient httpClient;

    public BackendClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAsync() 
    {
        var employees = await httpClient.GetFromJsonAsync<IEnumerable<Employee>>("employees");
        if (employees is null)
        {
            return Array.Empty<Employee>();
        }
        return employees;
    }

    public async Task<Employee?> GetEmployeeByIdAsync(Guid id) => 
        await httpClient.GetFromJsonAsync<Employee>($"employees/{id}");    
}