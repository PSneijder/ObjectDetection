using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ObjectDetection.WebApp.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected IActionResult InternalServerError(Exception exception)
        {
            Trace.WriteLine(exception);

            return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
        }
    }
}