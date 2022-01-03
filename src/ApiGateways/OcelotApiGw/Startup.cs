using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;

namespace OcelotApiGw
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOcelot()
				.AddCacheManager(configBuilder =>
				{
					configBuilder.WithDictionaryHandle();
				});

			services.AddHealthChecks()
				.AddUrlGroup(new Uri($"{Configuration["ApiSettings:CatalogUrl"]}/hc"), "Catalog.API", HealthStatus.Degraded)
				.AddUrlGroup(new Uri($"{Configuration["ApiSettings:BasketUrl"]}/hc"), "Basket.API", HealthStatus.Degraded)
				.AddUrlGroup(new Uri($"{Configuration["ApiSettings:OrderingUrl"]}/hc"), "Ordering.API", HealthStatus.Degraded);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync("Hello World!");
					endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
					{
						Predicate = _ => true,
						ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
					});
				});
			});

			await app.UseOcelot();
		}
	}
}
