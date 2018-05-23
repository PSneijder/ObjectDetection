using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace ObjectDetection.WebApp.Extensions
{
    internal static class StreamExtensions
    {
        public static IFormFile ToFormFile(this Stream stream, string name, string fileName)
        {
            using (stream)
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0L;

                return new FormFile(memoryStream, 0, memoryStream.Length, name, fileName);
            }
        }
    }
}