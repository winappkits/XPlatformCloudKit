/*
 * LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
 */

using System;
using Windows.UI.Popups;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using XPlatformCloudKit.DataServices;

namespace XPlatformCloudKit.Services
{
    public class AzureMobileServiceAuthenticator : IAzureMobileServiceAuthenticator
    {
        public MobileServiceUser User { get; private set; }

        public async Task Authenticate()
        {
            string errorMessage = String.Empty;

            while (User == null)
            {
                try
                {
                    User = await ((AzureMobileService)(ServiceLocator.AzureMobileService))
                        .MobileServiceClient
                        .LoginAsync(AppSettings.MobileServiceAuthProvider);
                }
                catch (InvalidOperationException e)
                {
                    errorMessage = "You must log in. Login Required";
                }

                if (!String.IsNullOrEmpty(errorMessage))
                {
#if WINDOWS_PHONE
                    System.Windows.MessageBox.Show(errorMessage);
#endif

#if NETFX_CORE
                    var dialog = new MessageDialog(errorMessage);
                    dialog.Commands.Add(new UICommand("OK"));
                    await dialog.ShowAsync();
#endif 
                }

                errorMessage = String.Empty;
            }
        }
    }
}
