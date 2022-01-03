using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfRunBasics.Services;
using WpfRunBasics.ViewModels;

namespace WpfRunBasics
{
	public class Bootstrapper : BootstrapperBase
	{
		public Bootstrapper()
		{
			Initialize();
		}

		public IConfiguration Configuration { get; private set; }

		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			var builder = new ConfigurationBuilder()
								.SetBasePath(Directory.GetCurrentDirectory())
								.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);		

			Configuration = builder.Build();

			Console.WriteLine(Configuration.GetConnectionString("BloggingDatabase"));
			Console.WriteLine(Configuration.GetConnectionString("BloggingDatabase"));
			Console.WriteLine(Configuration.GetConnectionString("BloggingDatabase"));

			var serviceCollection = new ServiceCollection();
			ConfigureServices(serviceCollection);

			//ServiceProvider = serviceCollection.BuildServiceProvider();

			//var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

			DisplayRootViewFor<ProductsViewModel>();
		}

		protected override void Configure()
		{

			base.Configure();
		}

		private void ConfigureServices(IServiceCollection services)
		{
			services.AddHttpClient<ICatalogService, CatalogService>(configureClient =>
			{
				configureClient.BaseAddress = new Uri(Configuration["ApiSettings:CatalogUrl]"]);
			});
			services.AddHttpClient<IBasketService, BasketService>(configureClient =>
			{
				configureClient.BaseAddress = new Uri(Configuration["ApiSettings:BasketUrl"]);
			});
			services.AddHttpClient<IOrderService, OrderService>(configureClient =>
			{
				configureClient.BaseAddress = new Uri(Configuration["ApiSettings:OrderingUrl"]);
			});

			services.AddTransient(typeof(MainWindow));
		}

		//private IServiceProvider ConfigureServices()
		//{
		//	IServiceCollection services = new ServiceCollection();
			
		//}
	}
}
