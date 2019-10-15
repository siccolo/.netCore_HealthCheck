using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
//
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//
using Microsoft.AspNetCore.Http;

namespace WebAPI_v1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHealthChecksUI()
                    .AddHealthChecks()
                    .AddCheck<Helpers.HealthCheck>("api")
                    /*
                        install-package AspNetcore.HealthChecks.Publisher.*
                        .AddApplicationInsightsPublisher()
                        .AddUrlGroup(new Uri("http://httpbin.org/status/200"))
                    */
                    ;
            services.AddControllers();

            /*
            //
            services.Configure<ApiSettings>(Configuration);
            services.AddTransient<IInfraApp, InfraApp>();
            
            //get instance infraapp:
            var provider = services.BuildServiceProvider();
            var infraapp = provider.GetService<IInfraApp>();
            //
            */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //  health checks
            /*
            app
                .UseHealthChecks("/health", new HealthCheckOptions()
                {
                    ResponseWriter = HealthCheckResponse
                })
                .UseHealthChecks("/healthui", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
                })
                //.UseHealthChecksUI(setup => { setup.ApiPath = "/health"; setup.UIPath = "/healthui"; })
                ;
                */
            //

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHealthChecksUI();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = HealthCheckResponse
                });
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
                });

                endpoints.MapHealthChecksUI(setup =>
                {
                    setup.UIPath = "/healthui"; // this is ui path in your browser
                    setup.ApiPath = "/health-ui-api"; // the UI ( spa app )  use this path to get information from the store ( this is NOT the healthz path, is internal ui api )
                });
                endpoints.MapControllers();
            });
        }

        #region HealthCheck output
        private static Task HealthCheckResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));
            return httpContext.Response.WriteAsync(     json.ToString(Formatting.Indented)  );
        }
        #endregion
    }
}
