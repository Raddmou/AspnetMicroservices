using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Shopping.Aggregator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Aggregator
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
			//IHttpClientFactory typed clients
			services.AddHttpClient<ICatalogService, CatalogService>(configureClient =>
			{
				configureClient.BaseAddress = new Uri(Configuration.GetValue<string>("ApiSettings:CatalogUrl"));
			});
			services.AddHttpClient<IBasketService, BasketService>(configureClient =>
			{
				configureClient.BaseAddress = new Uri(Configuration.GetValue<string>("ApiSettings:BasketUrl"));
			});
			services.AddHttpClient<IOrderService, OrderService>(configureClient =>
			{
				configureClient.BaseAddress = new Uri(Configuration.GetValue<string>("ApiSettings:OrderingUrl"));
			});

			services.AddHealthChecks()
				.AddUrlGroup(new Uri($"{Configuration["ApiSettings:CatalogUrl"]}/hc"), "Catalog.API", HealthStatus.Degraded)
				.AddUrlGroup(new Uri($"{Configuration["ApiSettings:BasketUrl"]}/hc"), "Basket.API", HealthStatus.Degraded)
				.AddUrlGroup(new Uri($"{Configuration["ApiSettings:OrderingUrl"]}/hc"), "Ordering.API", HealthStatus.Degraded);

			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shopping.Aggregator", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
		{
			logger.LogInformation(Configuration.GetValue<string>("ApiSettings:OrderingUrl"));
			logger.LogInformation(Configuration.GetValue<string>("ApiSettings:BasketUrl"));
			logger.LogInformation(Configuration.GetValue<string>("ApiSettings:OrderingUrl"));

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping.Aggregator v1"));
			}

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
				{
					Predicate = _ => true,
					ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
				});
			});
		}
	}
}
