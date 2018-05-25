using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ObjectDetection.WebApp.Controllers
{
    [Route("[controller]")]
    public sealed class ErrorsController : Controller
    {
        private readonly TelemetryClient _telemetryClient;

        public ErrorsController(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        [HttpGet]
        [Route("500")]
        public IActionResult Error()
        {
            var handlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            return View(handlerPathFeature);
        }
    }
}