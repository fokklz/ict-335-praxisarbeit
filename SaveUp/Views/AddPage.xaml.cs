using SaveUp.ViewModels;

namespace SaveUp.Views;

public partial class AddPage : ContentPage
{
	public AddPage(AddViewModel viewModels)
	{
		InitializeComponent();
		BindingContext = viewModels;
	}
}