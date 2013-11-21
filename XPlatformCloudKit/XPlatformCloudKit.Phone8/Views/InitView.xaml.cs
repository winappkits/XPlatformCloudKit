/*
 * LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
 */
using System.Windows;
using Cirrious.MvvmCross.WindowsPhone.Views;
using XPlatformCloudKit.Services;
using XPlatformCloudKit.ViewModels;

namespace XPlatformCloudKit.Views
{
    public partial class InitView : MvxPhonePage
    {
        public InitView()
        {
            InitializeComponent();
            Loaded += InitView_Loaded;
        }

        private async void InitView_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSettings.EnableMobileServiceAuth)
            {
                await ServiceLocator.AzureMobileServiceAuthenticator.Authenticate();
            }

            ((InitViewModel)DataContext).GotoItemsShowcaseCommand.Execute();
        }
    }
}