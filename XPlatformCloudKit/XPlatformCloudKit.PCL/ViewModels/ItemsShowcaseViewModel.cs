/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlatformCloudKit.Common;
using XPlatformCloudKit.DataServices;
using XPlatformCloudKit.Models;
using XPlatformCloudKit.Services;
using XPlatformCloudKit.Helpers;
using Cirrious.MvvmCross;
using Cirrious.MvvmCross.Plugins.File;
using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Plugins.Json;
using System.Diagnostics;
using System.Threading;
using Cirrious.CrossCore;
namespace XPlatformCloudKit.ViewModels
{
    public class ItemsShowcaseViewModel : MvxViewModel
    {
        private static Semaphore pool = new Semaphore(1, 1);
        MvxJsonConverter mvxJsonConverter = new MvxJsonConverter();
        IMvxFileStore fileStore = Mvx.Resolve<IMvxFileStore>();
        List<Item> items;
        List<IDataService> EnabledDataServices;


        #region Constructors
        public ItemsShowcaseViewModel()
        {
            // If we are running in the debugger, tell LoadItems() to ignore the cache in case we modified
            //  the RSS feeds or other content elements.  This way we are sure to see the desired content
            //  when iterating program changes in the debugger.
            LoadItems(Debugger.IsAttached);
        }

        public ItemsShowcaseViewModel(bool overrideCache)
        {
            //  If overrideCache is true, tell LoadItems() to ignore the cache, else if false, use the
            //  cache. This allows for direct control of cache availability during testing.
            LoadItems(overrideCache);
        }

        #endregion // Constructors

        #region Internal Methods
        /// <summary>
        /// Loads Items from our DataServices and sort into grouped enumerable
        /// </summary>
        private async void LoadItems(bool overrideCache = false)
        {
            IsBusy = true;

            if (AppSettings.EnableRemoteAppSettings)
            {
                RemoteAppSettingsService remoteAppSettingsService = new RemoteAppSettingsService();
                await remoteAppSettingsService.LoadRemoteAppSettings(overrideCache);
            }

            //Name may change after RemoteSettings enabled
            ApplicationName = AppSettings.ApplicationName;

            if (AppSettings.EnableRemoteUrlSourceService)
            {
                await RemoteUrlSourceService.GetRemoteUrlSources();
            }

            EnabledDataServices = DataServiceFactory.GetCurrentDataService();

            if (EnabledDataServices.Count == 0)
            {
                ServiceLocator.MessageService.ShowErrorAsync("No DataServices Enabled", "Application Error");
                return;
            }

            items = new List<Item>();
            await DoFetchDataServices(EnabledDataServices, overrideCache);

            //ItemGroups = new List<Group<Item>>(from item in items
            //                                   group item by item.Group into grp
            //                                   orderby grp.GetOrderPreference()
            //                                   select new Group<Item>(grp.Key, grp)).ToList();

            ItemGroups = new List<Group<Item>>(items.BuildQuery()).ToList(); //better way to handle LINQ queries

            IsBusy = false;

            if (LoadCompleted != null)
                LoadCompleted(this, EventArgs.Empty);

        }

        private Task DoFetchDataServices(List<IDataService> enabledDataServices, bool overrideCache = false)
        {
            IList<Task> tasks = new List<Task>();

            foreach (var dataService in enabledDataServices)
            {
                tasks.Add(
                    Task.Run(async () => await LoadDataService(dataService, overrideCache)));
            }
            
            return Task.WhenAll(tasks.ToArray());
        }

        private async Task LoadDataService(IDataService dataService, bool overrideCache = false)
        {
            List<Item> currentItems = new List<Item>();

            bool loadedFromCache = false;

            if (fileStore != null)
            {
                string lastRefreshText;
                if (fileStore.TryReadTextFile("LastRefresh-" + dataService.GetType().ToString(), out lastRefreshText))
                {
                    var lastRefreshTime = DateTime.Parse(lastRefreshText);
                    var timeSinceLastRefreshInMinutes = (DateTime.Now - lastRefreshTime).TotalMinutes;

                    //has cache expired?
                    if (overrideCache || timeSinceLastRefreshInMinutes > AppSettings.CacheIntervalInMinutes)
                    {
                        currentItems = await dataService.GetItems();
                    }
                    else //load from cache
                    {
                        string cachedItemsText;
                        pool.WaitOne();
                        try
                        {
                            if (fileStore.TryReadTextFile("CachedItems-" + dataService.GetType().ToString(), out cachedItemsText))
                            {
                                currentItems = mvxJsonConverter.DeserializeObject<List<Item>>(cachedItemsText);
                                loadedFromCache = true;
                            }
                        }
                        catch
                        {
                            ServiceLocator.MessageService.ShowErrorAsync("Error Deserializing " + dataService.GetType().ToString(), "Error loading cache");
                        }
                        finally
                        {
                            pool.Release();
                        }
                    }
                }
                else
                {
                    currentItems = await dataService.GetItems();
                }
            }

            try
            {
                if (!loadedFromCache && currentItems.Count > 0)
                {
                    if (fileStore.Exists("CachedItems-" + dataService.GetType().ToString()))
                        fileStore.DeleteFile("CachedItems-" + dataService.GetType().ToString());

                    if (fileStore.Exists("LastRefresh-" + dataService.GetType().ToString()))
                        fileStore.DeleteFile("LastRefresh-" + dataService.GetType().ToString());

                    fileStore.WriteFile("CachedItems-" + dataService.GetType().ToString(), mvxJsonConverter.SerializeObject(currentItems));
                    fileStore.WriteFile("LastRefresh-" + dataService.GetType().ToString(), DateTime.Now.ToString());
                }

                items.AddRange(currentItems);
            }
            catch
            {
                ServiceLocator.MessageService.ShowErrorAsync("Error retrieving items from " + dataService.GetType().ToString() + "\n\nPossible Causes:\nNo internet connection\nRemote Service unavailable", "Application Error");
            }

        }

        #endregion Internal Methods

        #region Public Properties
        private string applicationName;
        public string ApplicationName
        {
            set { applicationName = value; AppSettings.ApplicationName = value; RaisePropertyChanged(() => ApplicationName); }
            get { return applicationName; }
        }

        private Boolean isSearch;
        public Boolean IsSearch
        {
            get { return isSearch; }
            set { isSearch = value; RaisePropertyChanged(() => IsSearch); }
        }

        private List<Group<Item>> itemGroups;
        /// <summary>
        /// Gets or sets the grouping of items to display in the view.
        /// </summary>
        /// <value>
        /// The grouping of items to display in the view.
        /// </value>
        public List<Group<Item>> ItemGroups
        {
            get { return itemGroups; }
            set { itemGroups = value; RaisePropertyChanged(() => ItemGroups); }
        }

        private bool isBusy;
        /// <summary>
        /// Gets or sets the isBusy of the <see cref="AppListVM"/>.
        /// </summary>
        /// <value>
        /// The isBusy of the <c>AppListVM</c>.
        /// </value>
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; RaisePropertyChanged(() => IsBusy); }
        }

        private Item selectedItem;
        /// <summary>
        /// Gets or sets the selectedItem of the <see cref="ItemsShowcaseViewModel"/>.
        /// </summary>
        /// <value>
        /// The selectedItem of the <c>ItemsShowcaseViewModel</c>.
        /// </value>
        public Item SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (value != null)
                {
                    selectedItem = value;
                    RaisePropertyChanged(() => SelectedItem);
                    ItemSelectedCommand.Execute(selectedItem);
                }
            }
        }
        #endregion // Public Properties

        #region Commands
        private RelayCommand<Item> itemSelectedCommand;
        public RelayCommand<Item> ItemSelectedCommand
        {
            get
            {
                if (itemSelectedCommand == null)
                {
                    itemSelectedCommand = new RelayCommand<Item>(
                        (item) =>
                        {
                            AppState.SelectedItem = item;
                            AppState.SelectedGroup = ItemGroups.Where(x => x.Key == item.Group).ToList().First();
                            this.ShowViewModel(typeof(ItemDescriptionViewModel));
                        });
                }

                return itemSelectedCommand;
            }
        }

        private RelayCommand<object> refreshCommand;
        public RelayCommand<object> RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new RelayCommand<object>(
                        (item) =>
                        {
                            LoadItems(true);
                        });
                }

                return refreshCommand;
            }
        }

        private RelayCommand<object> clearSearch;
        public RelayCommand<object> ClearSearch
        {
            get
            {
                if (clearSearch == null)
                {
                    clearSearch = new RelayCommand<object>(
                        (item) =>
                        {
                            if (tempApplicationName != null)
                            {
                                ApplicationName = tempApplicationName;
                                tempApplicationName = null;
                            }
                            ItemGroups = temp;
                            temp = null;
                            IsSearch = false;
                        });
                }

                return clearSearch;
            }
        }

        private RelayCommand<string> searchCommand;
        public RelayCommand<string> SearchCommand
        {
            get
            {
                if (searchCommand == null)
                {
                    searchCommand = new RelayCommand<string>(
                        (searchTerm) =>
                        {
                            if (searchTerm != null && !searchTerm.Equals("") && ItemGroups != null && ItemGroups.Count > 0)
                            {
                                IsSearch = true;
                                if (tempApplicationName == null)
                                {
                                    tempApplicationName = ApplicationName;
                                    temp = ItemGroups;
                                }
                                ApplicationName = "Search Results";
                                List<Group<Item>> itemgroup = new List<Group<Item>>();
                                foreach (Group<Item> groups in temp)
                                {
                                    Group<Item> tempgroup = new Group<Item>();
                                    tempgroup.Key = groups[0].Group;
                                    foreach (Item item in groups)
                                    {
                                        if (item.Title.ToLower().Contains(searchTerm.ToLower()) || item.Subtitle.ToLower().Contains(searchTerm.ToLower()) || item.Description.ToLower().Contains(searchTerm.ToLower()))
                                        {
                                            tempgroup.Add(item);
                                        }
                                    }
                                    if (tempgroup.Count > 0)
                                        itemgroup.Add(tempgroup);
                                }
                                ItemGroups = itemgroup;
                                if (itemgroup.Count == 0)
                                {
                                    ServiceLocator.MessageService.ShowErrorAsync("We didn't find any results. Please try another query.", "No Results Found");
                                    ItemGroups = temp;
                                }
                            }
                            else if (tempApplicationName != null)
                            {
                                ApplicationName = tempApplicationName;
                                tempApplicationName = null;
                            }

                        });
                }

                return searchCommand;
            }
        }
        List<Group<Item>> temp;
        private string tempApplicationName;

        #endregion //Commands

        #region Events
        public delegate void LoadCompletedEventHandler(object sender, EventArgs e);
        public event LoadCompletedEventHandler LoadCompleted;
        #endregion
    }
}
