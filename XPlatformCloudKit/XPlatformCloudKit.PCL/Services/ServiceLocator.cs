/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlatformCloudKit.DataServices;

namespace XPlatformCloudKit.Services
{
    /// <summary>
    /// Singleton for storage and retrieval of service implementations
    /// </summary>
    public static class ServiceLocator
    {
        public static ILiveTileNotifyService LiveTileNotifyService { get; set; }
        public static IMessageService MessageService { get; set; }
        public static INavigationService NavigationService { get; set; }
        public static IDataService AzureMobileService { get; set; }
        public static IResourceFileService ResourceFileService { get; set; }
        public static IAzureMobileServiceAuthenticator AzureMobileServiceAuthenticator { get; set; }
    }
}
