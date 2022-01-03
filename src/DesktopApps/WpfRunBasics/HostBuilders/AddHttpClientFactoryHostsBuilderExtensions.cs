using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfRunBasics.Services;

namespace WpfRunBasics.HostBuilders
{
	public static class AddHttpClientFactoryHostsBuilderExtensions
	{
        public static IHostBuilder AddHttpClientFactory(this IHostBuilder host)
        {
            host.ConfigureServices((context, services) =>
            {
                services.AddHttpClient<ICatalogService, CatalogService>(configureClient =>
                {
                    configureClient.BaseAddress = new Uri(context.Configuration["ApiSettings:CatalogUrl]"]);
                });
                services.AddHttpClient<IBasketService, BasketService>(configureClient =>
                {
                    configureClient.BaseAddress = new Uri(context.Configuration["ApiSettings:BasketUrl"]);
                });
                services.AddHttpClient<IOrderService, OrderService>(configureClient =>
                {
                    configureClient.BaseAddress = new Uri(context.Configuration["ApiSettings:OrderingUrl"]);
                });
            });

            return host;
        }
    }
}
