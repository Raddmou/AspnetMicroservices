using Caliburn.Micro;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WpfRunBasics.Models;

namespace WpfRunBasics.ViewModels
{
	public class ProductsViewModel : ViewModelBase
	{
		private ObservableCollection<CatalogModel> _products = null;
		private CatalogModel _selectedProduct = null;


		public ProductsViewModel()
		{
			this._products = new ObservableCollection<CatalogModel>();
		}

		public ObservableCollection<CatalogModel> Products 
		{
			get
			{
				return this._products;
			}
			set
			{
				this._products = value;
			}
		}

		public CatalogModel SelectedProduct
		{
			get
			{
				return this._selectedProduct;
			}
			set
			{
				this._selectedProduct = value;
			}
		}
	}
}