using SaveUp.ViewModels;

namespace SaveUp.Views;

public partial class HomePage : ContentPage
{
	private readonly HomeViewModel _viewModel;

	public HomePage(HomeViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();

		BindingContext = _viewModel;
	}

	protected override void OnAppearing()
    {
        base.OnAppearing();
		_viewModel.LoadItems();
    }
}