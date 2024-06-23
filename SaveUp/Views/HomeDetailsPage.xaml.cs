using SaveUp.ViewModels;

namespace SaveUp.Views;

public partial class HomeDetailsPage : ContentPage
{
	public HomeDetailsPage(HomeDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}