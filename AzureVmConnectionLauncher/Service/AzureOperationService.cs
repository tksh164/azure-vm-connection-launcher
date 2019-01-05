using System.Collections.ObjectModel;
using AzureVmConnectionLauncher.Model;
using AzureVmConnectionLauncher.ViewModel;

namespace AzureVmConnectionLauncher.Service
{
    public interface IAzureOperationService
    {
        ObservableCollection<AzureSubscriptionViewModel> ConnectAccount();
        ObservableCollection<AzureResourceGroupViewModel> GetResourceGroups(AzureSubscription subscription);
        ObservableCollection<AzureVirtualMachineViewModel> GetVirtualMachines(string subscriptionId, string resourceGroupName);
    }

    internal class AzureOperationService : IAzureOperationService
    {
        public ObservableCollection<AzureSubscriptionViewModel> ConnectAccount()
        {
            var placeholderResourceGroupItem = new AzureResourceGroupViewModel()
            {
                ResourceGroup = null,
                IsPlaceholder = true,
                IsExpanded = false,
                VirtualMachines = null,
            };

            var subscriptions = AzureOperation.ConnectAccount();
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

        public ObservableCollection<AzureResourceGroupViewModel> GetResourceGroups(AzureSubscription subscription)
        {
            var placeholderVirtualMachineItem = new AzureVirtualMachineViewModel()
            {
                VirtualMachine = null,
                IsPlaceholder = true,
            };

            var resourceGroups = AzureOperation.GetResourceGroups(subscription.SubscriptionId);
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

        public ObservableCollection<AzureVirtualMachineViewModel> GetVirtualMachines(string subscriptionId, string resourceGroupName)
        {
            var virtualMachines = AzureOperation.GetVirtualMachines(subscriptionId, resourceGroupName);
            var virtualMachineTreeViewItems = new ObservableCollection<AzureVirtualMachineViewModel>();
            foreach (var virtualMachine in virtualMachines)
            {
                var virtualMachineItem = new AzureVirtualMachineViewModel()
                {
                    VirtualMachine = virtualMachine,
                    ConnectionDestinations = new ObservableCollection<ConnectionDestinationViewModel>(),
                };

                foreach (var destination in virtualMachine.ConnectionDestinations)
                {
                    virtualMachineItem.ConnectionDestinations.Add(new ConnectionDestinationViewModel
                    {
                        VirtualMachine = virtualMachine,
                        ConnectionDestination = destination,
                    });
                }

                virtualMachineTreeViewItems.Add(virtualMachineItem);
            }

            return virtualMachineTreeViewItems;
        }
    }
}
