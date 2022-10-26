using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HostnameController : ControllerBase
{
    [HttpGet]
    public string Get() => Dns.GetHostName();
}
