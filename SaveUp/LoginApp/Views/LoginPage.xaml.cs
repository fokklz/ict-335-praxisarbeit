using SaveUp.LoginApp.ViewModels;

namespace SaveUp.LoginApp.Views;

public partial class LoginPage : ContentPage
{

	private readonly LoginViewModel _viewModel;

	public LoginPage(LoginViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}

	protected override void OnAppearing()
    {
        base.OnAppearing();
		_viewModel.ResetLoginState();
    }
}