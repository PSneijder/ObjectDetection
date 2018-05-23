using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ObjectDetection.WebApp.Extensions;

namespace ObjectDetection.WebApp.Managers
{
    public sealed class BlobManager
    {
        private readonly IHostingEnvironment _env;
        private readonly string _baseUri;

        public BlobManager(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _baseUri = config["BaseUri"];
        }

        public Task<string[]> Upload(Stream stream)
        {
            string name = Guid.NewGuid().ToString("N");
            string fileName = $"{name}.jpeg";

            return Upload(new[] { stream.ToFormFile(name, fileName) } );
        }

        public Task<string[]> Upload(IFormFile[] files)
        {
            var urls = new List<string>();

            foreach (IFormFile file in files)
            {
                string fileName = GetUniqueFileName(file.FileName);

                string rootPath = GetRootPath();
                string fullPath = Path.Combine(rootPath, fileName);

                if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

                using (Stream stream = file.OpenReadStream())
                {
                    CopyTo(stream, fullPath);
                }
                
                UriBuilder builder =
                    new UriBuilder(new Uri(_baseUri))
                    {
                        Path = $"{Constants.UploadPath}/{fileName}"
                    };

                urls.Add(builder.ToString());
            }

            return Task.FromResult(urls.ToArray());
        }

        public Task<string[]> ListBlobs()
        {
            var urls = new List<string>();

            string path = GetRootPath();

            foreach (string file in Directory.EnumerateFiles(path, "*.*").AsParallel())
            {
                string fileName = Path.GetFileName(file);

                UriBuilder builder =
                    new UriBuilder(new Uri(_baseUri))
                    {
                        Path = $"{Constants.UploadPath}/{fileName}"
                    };

                urls.Add(builder.ToString());
            }

            return Task.FromResult(urls.ToArray());
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);

            fileName = Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString("N").Substring(0, 4)
                   + Path.GetExtension(fileName);

            return fileName.ToLowerInvariant();
        }

        private string GetRootPath()
        {
            return Path.Combine(_env.ContentRootPath, Constants.UploadFolder);
        }

        private void CopyTo(Stream file, string fullPath)
        {
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
        }
    }
}