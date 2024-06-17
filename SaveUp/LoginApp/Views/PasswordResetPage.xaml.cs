using SaveUp.LoginApp.ViewModels;

namespace SaveUp.LoginApp.Views;

public partial class PasswordResetPage : ContentPage
{
	public PasswordResetPage(PasswordResetViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}