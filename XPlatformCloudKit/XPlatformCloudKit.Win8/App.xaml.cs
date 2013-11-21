/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.File.WindowsStore;
using Cirrious.MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XPlatformCloudKit.DataServices;
using XPlatformCloudKit.Helpers;
using XPlatformCloudKit.Services;
using XPlatformCloudKit.Views;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace XPlatformCloudKit.Win8
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            if (AppSettings.UseLightThemeForWindows8)
                this.RequestedTheme = ApplicationTheme.Light;
            else
                this.RequestedTheme = ApplicationTheme.Dark;
            
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            DispatcherHelper.Initialize();

            ServiceLocator.MessageService = new MessageService();
            ServiceLocator.LiveTileNotifyService = new LiveTileNotifyService();
            ServiceLocator.NavigationService = new NavigationService();
            ServiceLocator.AzureMobileService = new AzureMobileService();
            ServiceLocator.ResourceFileService = new ResourceFileService();
            ServiceLocator.AzureMobileServiceAuthenticator = new AzureMobileServiceAuthenticator();

            //Using MVVM Cross Container
            var iocProvider = MvxSimpleIoCContainer.Initialise();
            Mvx.RegisterSingleton<IMvxFileStore>(new MvxWindowsStoreBlockingFileStore());

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            //if (rootFrame.Content == null)
            //{
            //    // When the navigation stack isn't restored navigate to the first page,
            //    // configuring the new page by passing required information as a navigation
            //    // parameter
            //    if (!rootFrame.Navigate(typeof(ItemsShowcaseView), args.Arguments))
            //    {
            //        throw new Exception("Failed to create initial page");
            //    }
            //}

            //MVVMCross setup
            var setup = new Setup(rootFrame);
            setup.Initialize();

            //MVVMCross setup
            var start = Mvx.Resolve<IMvxAppStart>();
            start.Start();

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
