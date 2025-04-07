using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace SocialApp
{
    public sealed partial class HomeScreen : Page
    {

        public HomeScreen()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TopBar.SetFrame(this.Frame);
            TopBar.SetHome();
        }
    }
}
