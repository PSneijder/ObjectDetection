using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ObjectDetection.WebApp.Managers;
using ObjectDetection.WebApp.ViewModels;

namespace ObjectDetection.WebApp.Controllers
{
    public sealed class BlobsController : ControllerBase
    {
        private readonly BlobManager _manager;

        public BlobsController(BlobManager manager)
        {
            _manager = manager;
        }

        public async Task<IActionResult> Index()
        {
            var blobs = await _manager.ListBlobs();
            var viewModel = new BlobViewModel
            {
                Blobs = blobs
            };

            return View(viewModel);
        }
    }
}