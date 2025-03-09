using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Runtime.InteropServices;

namespace HelloWorldApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var frameworkDescription = RuntimeInformation.FrameworkDescription;
            var appVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown";
            
            var versionInfo = new
            {
                FrameworkVersion = frameworkDescription,
                ApplicationVersion = appVersion,
                DotNetVersion = Environment.Version.ToString(),
                OperatingSystem = RuntimeInformation.OSDescription
            };
            
            return Ok(versionInfo);
        }
    }
}