using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ObjectDetection.WebApp.Extensions;
using ObjectDetection.WebApp.Managers;
using ObjectDetection.WebApp.Models;
using ObjectDetection.WebApp.ViewModels;

namespace ObjectDetection.WebApp.Controllers
{
    public sealed class DevicesController : ControllerBase
    {
        private readonly DeviceManager _manager;

        public DevicesController(DeviceManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var devices = await _manager.ListDevices();
            var viewModel = new DeviceViewModel
            {
                Devices = devices
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSnapshot(string deviceId)
        {
            await _manager.SendDeviceCommand(deviceId, Commands.CreateSnapshot.Name());

            return RedirectToAction("Index", "Blobs");
        }
    }
}