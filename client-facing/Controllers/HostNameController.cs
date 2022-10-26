using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace MultiArchApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HostNameController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return Dns.GetHostName();
    }
}
