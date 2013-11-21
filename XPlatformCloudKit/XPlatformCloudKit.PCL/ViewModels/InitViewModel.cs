/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/
using Cirrious.MvvmCross.ViewModels;

namespace XPlatformCloudKit.ViewModels
{
    public class InitViewModel : MvxViewModel
    {
        private MvxCommand _gotoItemsShowcaseCommand;
        public MvxCommand GotoItemsShowcaseCommand
        {
            get
            {
                _gotoItemsShowcaseCommand = _gotoItemsShowcaseCommand ?? new MvxCommand(GotoItemsShowcaseCommandExecute);
                return _gotoItemsShowcaseCommand;
            }
        }

        private void GotoItemsShowcaseCommandExecute()
        {
            this.ShowViewModel<ItemsShowcaseViewModel>();
        }
    }
}
