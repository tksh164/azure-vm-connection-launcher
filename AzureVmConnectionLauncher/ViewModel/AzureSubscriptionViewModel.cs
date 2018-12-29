using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
                            service.SetCurrentSubscription(Subscription.SubscriptionId);
                            ResourceGroups = service.GetResourceGroups(Subscription);
                        });
                    }
                }

                RaisePropertyChanged(nameof(IsExpanded));
            }
        }

        public string DisplayText
        {
            get
            {
                return string.Format("{0} ({1})", Subscription.Name, Subscription.SubscriptionId);
            }
        }
    }
}
