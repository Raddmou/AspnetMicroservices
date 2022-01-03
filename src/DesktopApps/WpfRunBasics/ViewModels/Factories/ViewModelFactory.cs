using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfRunBasics.State.Navigators;

namespace WpfRunBasics.ViewModels.Factories
{
	public class ViewModelFactory : IViewModelFactory
	{
        private readonly CreateViewModel<ProductsViewModel> _createProductsViewModel;
        private readonly CreateViewModel<BasketViewModel> _createBasketViewModel;

		public ViewModelFactory(CreateViewModel<ProductsViewModel> createProductsViewModel, CreateViewModel<BasketViewModel> createBasketViewModel)
		{
			_createProductsViewModel = createProductsViewModel;
			_createBasketViewModel = createBasketViewModel;
		}

		public ViewModelBase CreateViewModel(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.Products:
                    return _createProductsViewModel();
                case ViewType.Basket:
                    return _createBasketViewModel();
                default:
                    throw new ArgumentException("The ViewType does not have a ViewModel.", "viewType");
            }
        }
    }
}
