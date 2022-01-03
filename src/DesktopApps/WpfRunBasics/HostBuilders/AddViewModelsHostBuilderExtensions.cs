using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfRunBasics.State.Navigators;
using WpfRunBasics.ViewModels;
using WpfRunBasics.ViewModels.Factories;

namespace WpfRunBasics.HostBuilders
{
    public static class AddViewModelsHostBuilderExtensions
    {
        public static IHostBuilder AddViewModels(this IHostBuilder host)
        {
            host.ConfigureServices(services =>
            {
                services.AddTransient<MainViewModel>();
                services.AddTransient<ProductsViewModel>();
                services.AddTransient<BasketViewModel>();

                services.AddSingleton<CreateViewModel<ProductsViewModel>>(services => () => services.GetRequiredService<ProductsViewModel>());
                services.AddSingleton<CreateViewModel<BasketViewModel>>(services => () => services.GetRequiredService<BasketViewModel>());

                services.AddSingleton<IViewModelFactory, ViewModelFactory>();

                services.AddSingleton<ViewModelDelegateRenavigator<ProductsViewModel>>();
            });

            return host;
        }
    }
}