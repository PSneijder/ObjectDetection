using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ObjectDetection.WebApp.Extensions;
using ObjectDetection.WebApp.Managers;
using ObjectDetection.WebApp.Models;

namespace ObjectDetection.WebApp.Controllers
{
    [Route("api/[controller]")]
    public sealed class DevicesController
        : ControllerBase
    {
        private readonly DeviceManager _manager;

        public DevicesController(DeviceManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetDevices()
        {
            try
            {
                Device[] devices = await _manager.ListDevices();

                return Ok(devices);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPost("{deviceId}/commands")]
        public async Task<IActionResult> SendCommand(string deviceId, [FromBody] Commands command)
        {
            try
            {
                await _manager.SendDeviceCommand(deviceId, command.Name());

                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPost("{deviceId}/messages")]
        public async Task<IActionResult> SendMessage(string deviceId, [FromBody] string message)
        {
            try
            {
                await _manager.SendDeviceMessage(deviceId, message);

                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet("commands")]
        public IActionResult GetCommands()
        {
            try
            {
                return Ok(Enum.GetNames(typeof(Commands)));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}