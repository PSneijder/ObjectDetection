using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ObjectDetection.WebApp.Extensions;
using ObjectDetection.WebApp.Hubs;
using ObjectDetection.WebApp.Managers;
using Swashbuckle.AspNetCore.Swagger;

namespace ObjectDetection.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DeviceManager>();
            services.AddSingleton<BlobManager>();

            services.AddMvc();
            services.AddSignalR();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info {Title = "ObjectDetection"});
                options.DescribeAllEnumsAsStrings();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug();
            loggerFactory.AddAzureWebAppDiagnostics();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ObjectDetection V1");
                options.RoutePrefix = string.Empty;
                options.EnableFilter();
            });

            app.UseStaticFiles();
            app.UseStaticFilesIn(env, Constants.UploadPath);

            app.UseMvc();
            app.UseSignalR(builder => builder.MapHub<DeviceHub>("/deviceHub"));
        }
    }
}