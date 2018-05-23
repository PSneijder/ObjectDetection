﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ObjectDetection.WebApp.Filters;
using ObjectDetection.WebApp.Managers;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ObjectDetection.WebApp.Controllers
{
    [Route("api/[controller]")]
    public sealed class BlobsController
        : ControllerBase
    {
        private readonly BlobManager _manager;

        public BlobsController(BlobManager manager)
        {
            _manager = manager;
        }

        [SwaggerOperationFilter(typeof(FormFileOperationFilter))]
        [HttpPost]
        public async Task<IActionResult> UploadBlob(IFormFile file)
        {
            try
            {
                string[] urls;

                if (file == null)
                {
                    urls = await _manager.Upload(Request.Body);
                }
                else
                {
                    urls = await _manager.Upload(new[] { file });
                }

                return Ok(urls);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBlobs()
        {
            try
            {
                string[] urls = await _manager.ListBlobs();

                return Ok(urls);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}