using SaveUp.LoginApp;

namespace SaveUp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new LoginAppShell();
        }
    }
}
