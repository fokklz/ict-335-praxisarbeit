using SaveUp.Views;

namespace SaveUp.Components;

public partial class Footer : ContentView
{
	public Command GotoImprintCommand { get; }

    public Command GotoPrivacyPolicyCommand { get; }

	public Footer()
	{
		InitializeComponent();

		GotoImprintCommand = new Command(GotoImprint);
		GotoPrivacyPolicyCommand = new Command(GotoPrivacyPolicy);

		BindingContext = this;
	}

    private void GotoImprint()
	{
		var imprintPage = new Imprint();
		Shell.Current.Navigation.PushAsync(imprintPage);
	}

    private void GotoPrivacyPolicy()
    {
        var privacyPolicyPage = new PrivacyPolicy();
        Shell.Current.Navigation.PushAsync(privacyPolicyPage);
    }
}