using SaveUp.Views;

namespace SaveUp.Components;

public partial class Footer : ContentView
{
	/// <summary>
	/// Command to navigate to the imprint page
	/// </summary>
	public Command GotoImprintCommand { get; }

	/// <summary>
	/// Command to navigate to the privacy policy page
	/// </summary>
    public Command GotoPrivacyPolicyCommand { get; }

	public Footer()
	{
		InitializeComponent();

		GotoImprintCommand = new Command(GotoImprint);
		GotoPrivacyPolicyCommand = new Command(GotoPrivacyPolicy);

		BindingContext = this;
	}

	/// <summary>
	/// Creates a new instance of the imprint page and navigates to it
	/// </summary>
    private void GotoImprint()
	{
		var imprintPage = new Imprint();
		Shell.Current.Navigation.PushAsync(imprintPage);
	}

	/// <summary>
	/// Creates a new instance of the privacy policy page and navigates to it
	/// </summary>
    private void GotoPrivacyPolicy()
    {
        var privacyPolicyPage = new PrivacyPolicy();
        Shell.Current.Navigation.PushAsync(privacyPolicyPage);
    }
}