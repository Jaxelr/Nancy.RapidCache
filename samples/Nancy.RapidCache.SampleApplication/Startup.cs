using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nancy.Owin;

namespace Asp.Net.Example
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddLogging(opt =>
            {
                opt.AddConsole();
                opt.AddDebug();
            });

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOwin(x => x.UseNancy());
        }
    }
}
