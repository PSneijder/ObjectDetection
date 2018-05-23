using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ObjectDetection.WebApp.Filters
{
    sealed class FormFileOperationFilter : IOperationFilter
    {
        private const string FormDataMimeType = "multipart/form-data";
        private static readonly string[] FormFilePropertyNames = typeof(IFormFile).GetTypeInfo().DeclaredProperties.Select(p => p.Name).ToArray();

        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ParameterDescriptions.Any(p => p.ModelMetadata.ContainerType == typeof(IFormFile) || p.ModelMetadata.UnderlyingOrModelType == typeof(IFormFile)))
            {
                NonBodyParameter[] formFileParameters = operation
                    .Parameters
                    .OfType<NonBodyParameter>()
                    .Where(p => FormFilePropertyNames.Contains(p.Name) || p.Name == "file")
                    .ToArray();

                int index = operation.Parameters.IndexOf(formFileParameters.First());

                foreach (NonBodyParameter formFileParameter in formFileParameters)
                {
                    operation.Parameters.Remove(formFileParameter);
                }

                string formFileParameterName = context
                    .ApiDescription
                    .ActionDescriptor
                    .Parameters
                    .Where(p => p.ParameterType == typeof(IFormFile))
                    .Select(p => p.Name)
                    .First();

                NonBodyParameter parameter = new NonBodyParameter()
                {
                    Name = formFileParameterName,
                    In = "formData",
                    Description = "The file to upload.",
                    Required = true,
                    Type = "file"
                };

                operation.Parameters.Insert(index, parameter);

                if (!operation.Consumes.Contains(FormDataMimeType))
                {
                    operation.Consumes.Add(FormDataMimeType);
                }
            }
        }
    }
}