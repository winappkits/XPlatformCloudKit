/*
 * LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
 */
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace XPlatformCloudKit.Services
{
    /// <summary>
    /// Adaptor to provide AMS Authentication
    /// </summary>
    public interface IAzureMobileServiceAuthenticator
    {
        MobileServiceUser User { get; }

        Task Authenticate();
    }
}
