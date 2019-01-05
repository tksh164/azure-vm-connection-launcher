using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using AzureVmConnectionLauncher.Model;
using AzureVmConnectionLauncher.Service;

namespace AzureVmConnectionLauncher.ViewModel
{
    public class AzureSubscriptionViewModel : ViewModelBase
    {
        public AzureSubscription Subscription { get; set; }

        private ObservableCollection<AzureResourceGroupViewModel> resourceGroups;
        public ObservableCollection<AzureResourceGroupViewModel> ResourceGroups
        {
            get
            {
                return resourceGroups;
            }
            set
            {
                resourceGroups = value;
                RaisePropertyChanged(nameof(ResourceGroups));
            }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                isExpanded = value;

                if (isExpanded)
                {
                    var resourceGroup = ResourceGroups.First();
                    if (resourceGroup.IsPlaceholder)
                    {
                        Task.Run(() => {
                            var service = new AzureOperationService();
                            ResourceGroups = service.GetResourceGroups(Subscription);
                        });
                    }
                }

                RaisePropertyChanged(nameof(IsExpanded));
            }
        }

        public string MainDisplayText
        {
            get
            {
                return Subscription.Name;
            }
        }

        public string SubDisplayText
        {
            get
            {
                return Subscription.SubscriptionId;
            }
        }

        public Visibility SubDisplayTextVisibility
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SubDisplayText))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
    }
}
