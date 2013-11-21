/*
 * LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using XPlatformCloudKit.Common;
using XPlatformCloudKit.Helpers;
using XPlatformCloudKit.Models;
using XPlatformCloudKit.Services;
using XPlatformCloudKit.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace XPlatformCloudKit.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemsShowcaseView : LayoutAwarePage
    {
        public ItemsShowcaseView()
        {
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += ShowPrivacyPolicy;
            DataTransferManager.GetForCurrentView().DataRequested += ShareLinkHandler;
            Loaded += ItemsShowcaseView_Loaded;

            Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted += searchPane_QuerySubmitted;
            Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().ShowOnKeyboardInput = true;
        }
        
        void searchPane_QuerySubmitted(Windows.ApplicationModel.Search.SearchPane sender, Windows.ApplicationModel.Search.SearchPaneQuerySubmittedEventArgs args)
        {
            SearchButton.Command.Execute(sender.QueryText);
        }
                
        private void ShareLinkHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            var deferral = request.GetDeferral();
            request.Data.Properties.Title = AppSettings.ApplicationName;
            request.Data.SetText("Check out this awesome app in the Windows Store!");
            //request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Logo.png")));
            deferral.Complete();
        }

        // Method to add the privacy policy to the settings charm
        private void ShowPrivacyPolicy(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand privacyPolicyCommand = new SettingsCommand("privacyPolicy", "Privacy Policy", (x) => { LaunchPrivacyPolicyUrl(); });
            args.Request.ApplicationCommands.Add(privacyPolicyCommand);
        }

        // Method to launch the url of the privacy policy
        async void LaunchPrivacyPolicyUrl()
        {
            Uri privacyPolicyUrl = new Uri(AppSettings.PrivacyPolicyUrl);
            var result = await Windows.System.Launcher.LaunchUriAsync(privacyPolicyUrl);
        }

        void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Hack to allow Semantic Zoom to update properly
            //WARNING this can cause strange exception on Navigation if page not cached
            if (e.PropertyName == "ItemGroups") 
                ZoomedOutGroupGridView.ItemsSource = groupedItemsViewSource.View.CollectionGroups;
        }

        async void ItemsShowcaseView_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSettings.EnableMobileServiceAuth) { await ServiceLocator.AzureMobileServiceAuthenticator.Authenticate(); }

            ((ItemsShowcaseViewModel)DataContext).PropertyChanged += vm_PropertyChanged; 

            //Cache loads so fast if called from constructor that property changed is not fired
            if (groupedItemsViewSource.View != null && groupedItemsViewSource.View.CollectionGroups != null)
                ZoomedOutGroupGridView.ItemsSource = groupedItemsViewSource.View.CollectionGroups;
        }


    }
}
