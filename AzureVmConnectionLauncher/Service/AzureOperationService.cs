using System.Collections.ObjectModel;
using AzureVmConnectionLauncher.Model;
using AzureVmConnectionLauncher.ViewModel;

namespace AzureVmConnectionLauncher.Service
{
    public interface IAzureOperationService
    {
        void ConnectAccount();
        ObservableCollection<AzureSubscriptionViewModel> GetSubscriptions();
        void SetCurrentSubscription(string subscriptionId);
        ObservableCollection<AzureResourceGroupViewModel> GetResourceGroups(AzureSubscription subscription);
        ObservableCollection<AzureVirtualMachineViewModel> GetVirtualMachines(string resourceGroupName);
    }

    internal class AzureOperationService : IAzureOperationService
    {
        public void ConnectAccount()
        {
            AzureOperation.ConnectAccount();
        }

        public ObservableCollection<AzureSubscriptionViewModel> GetSubscriptions()
        {
            var placeholderResourceGroupItem = new AzureResourceGroupViewModel()
            {
                ResourceGroup = null,
                IsPlaceholder = true,
                IsExpanded = false,
                VirtualMachines = null,
            };

            var subscriptions = AzureOperation.GetSubscriptions();
            var subscriptionTreeViewItems = new ObservableCollection<AzureSubscriptionViewModel>();
            foreach (var subscription in subscriptions)
            {
                var subscriptionItem = new AzureSubscriptionViewModel()
                {
                    Subscription = subscription,
                    IsExpanded = false,
                    ResourceGroups = new ObservableCollection<AzureResourceGroupViewModel>(),
                };
                subscriptionItem.ResourceGroups.Add(placeholderResourceGroupItem);
                subscriptionTreeViewItems.Add(subscriptionItem);
            }

            return subscriptionTreeViewItems;
        }

        public void SetCurrentSubscription(string subscriptionId)
        {
            AzureOperation.SetCurrentSubscription(subscriptionId);
        }

        public ObservableCollection<AzureResourceGroupViewModel> GetResourceGroups(AzureSubscription subscription)
        {
            var placeholderVirtualMachineItem = new AzureVirtualMachineViewModel()
            {
                VirtualMachine = null,
                IsPlaceholder = true,
            };

            var resourceGroups = AzureOperation.GetResourceGroups();
            var resourceGroupTreeViewItems = new ObservableCollection<AzureResourceGroupViewModel>();
            foreach (var resourceGroup in resourceGroups)
            {
                var resourceGroupItem = new AzureResourceGroupViewModel()
                {
                    Subscription = subscription,
                    ResourceGroup = resourceGroup,
                    IsExpanded = false,
                    VirtualMachines = new ObservableCollection<AzureVirtualMachineViewModel>(),
                };
                resourceGroupItem.VirtualMachines.Add(placeholderVirtualMachineItem);
                resourceGroupTreeViewItems.Add(resourceGroupItem);
            }

            return resourceGroupTreeViewItems;
        }

        public ObservableCollection<AzureVirtualMachineViewModel> GetVirtualMachines(string resourceGroupName)
        {
            var virtualMachines = AzureOperation.GetVirtualMachines(resourceGroupName);
            var virtualMachineTreeViewItems = new ObservableCollection<AzureVirtualMachineViewModel>();
            foreach (var virtualMachine in virtualMachines)
            {
                var virtualMachineItem = new AzureVirtualMachineViewModel()
                {
                    VirtualMachine = virtualMachine,
                    IpAddressFqdnPairs = new ObservableCollection<IpAddressFqdnPairViewModel>(),
                };

                foreach (var pair in virtualMachine.IpAddressFqdnPairs)
                {
                    virtualMachineItem.IpAddressFqdnPairs.Add(new IpAddressFqdnPairViewModel {
                        VirtualMachine = virtualMachine,
                        IpAddressFqdnPair = pair,
                    });
                }

                virtualMachineTreeViewItems.Add(virtualMachineItem);
            }

            return virtualMachineTreeViewItems;
        }
    }
}
