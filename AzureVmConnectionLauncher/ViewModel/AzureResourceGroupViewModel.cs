﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using AzureVmConnectionLauncher.Model;
using AzureVmConnectionLauncher.Service;

namespace AzureVmConnectionLauncher.ViewModel
{
    public class AzureResourceGroupViewModel : ViewModelBase
    {
        public AzureSubscription Subscription { get; set; }

        public AzureResourceGroup ResourceGroup { get; set; }

        private ObservableCollection<AzureVirtualMachineViewModel> virtualMachines;
        public ObservableCollection<AzureVirtualMachineViewModel> VirtualMachines
        {
            get
            {
                return virtualMachines;
            }
            set
            {
                virtualMachines = value;
                RaisePropertyChanged(nameof(VirtualMachines));
            }
        }

        public bool IsPlaceholder { get; set; }

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
                    var virtualMachine = VirtualMachines.First();
                    if (virtualMachine.IsPlaceholder)
                    {
                        Task.Run(() => {
                            var service = new AzureOperationService();
                            service.SetCurrentSubscription(Subscription.SubscriptionId);
                            VirtualMachines = service.GetVirtualMachines(ResourceGroup.ResourceGroupName);
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
                if (IsPlaceholder)
                {
                    return "Loading resource groups...";
                }
                else
                {
                    return string.Format("{0} ({1})", ResourceGroup.ResourceGroupName, ResourceGroup.Location);
                }
            }
        }
    }
}
