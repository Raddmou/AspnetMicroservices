using WpfRunBasics.State.Navigators;

namespace WpfRunBasics.ViewModels.Factories
{
	public interface IViewModelFactory
    {
        ViewModelBase CreateViewModel(ViewType viewType);
    }
}
