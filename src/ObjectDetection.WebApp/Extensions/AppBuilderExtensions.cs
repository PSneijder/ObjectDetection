using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace ObjectDetection.WebApp.Extensions
{
    internal static class AppBuilderExtensions
    {
        public static void UseStaticFilesIn(this IApplicationBuilder app, IHostingEnvironment env, string path)
        {
            string fileProvider = Path.Combine(env.ContentRootPath, Constants.UploadFolder);
            if (!Directory.Exists(fileProvider)) Directory.CreateDirectory(fileProvider);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider),
                RequestPath = new PathString(Constants.UploadPath)
            });
        }
    }
}