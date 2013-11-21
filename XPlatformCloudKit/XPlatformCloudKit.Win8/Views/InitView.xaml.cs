/*
 * LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
 */
using Windows.UI.Xaml;
using XPlatformCloudKit.Common;
using XPlatformCloudKit.Services;
using XPlatformCloudKit.ViewModels;

namespace XPlatformCloudKit.Views
{
    public sealed partial class InitView : LayoutAwarePage
    {
        public InitView()
        {
            this.InitializeComponent();
            this.Loaded += InitView_OnLoaded;
        }

        private async void InitView_OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (AppSettings.EnableMobileServiceAuth)
            {
                await ServiceLocator.AzureMobileServiceAuthenticator.Authenticate();
            }

            ((InitViewModel)DataContext).GotoItemsShowcaseCommand.Execute();
        }
    }
}
