using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MultiArchApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HostNameController : ControllerBase
{
    private readonly IConfiguration _config;
    // private readonly HttpClient httpClient;
    // private readonly IHostnameService hostnameService;

    public HostNameController(IConfiguration config
    // , HttpClient httpClient, IHostnameService hostnameService
    )
    {
        _config = config;
        // this.httpClient = httpClient;
        // this.hostnameService = hostnameService;
    }

    [HttpGet]
    public async Task<string> Get()
    {
        var baseUriString = _config.GetValue<string>("BackendApiBaseAddress");
        var baseUri = new Uri(baseUriString);
        var client = new HttpClient
        {
            BaseAddress = baseUri
        };

        return await client.GetStringAsync("hostname");
    }
}

public interface IHostnameService
{
    Task<string> GetHostName();
}

public class HostNameService : IHostnameService
{
    private readonly HttpClient httpClient;

    public HostNameService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public Task<string> GetHostName()
    {
        return httpClient.GetStringAsync("hostname");
    }
}
