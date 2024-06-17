using SaveUp.LoginApp.ViewModels;

namespace SaveUp.LoginApp.Views;

public partial class RegisterPage : ContentPage
{
	private readonly RegisterViewModel _viewModel;

	public RegisterPage(RegisterViewModel viewModel)
	{
		_viewModel = viewModel;
		InitializeComponent();
		BindingContext = _viewModel;
	}

	protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ResetRegisterState();
    }
}