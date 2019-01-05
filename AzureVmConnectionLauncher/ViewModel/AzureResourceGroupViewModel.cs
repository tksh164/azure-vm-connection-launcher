using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
                            VirtualMachines = service.GetVirtualMachines(Subscription.SubscriptionId, ResourceGroup.ResourceGroupName);
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
                if (IsPlaceholder)
                {
                    return "Loading resource groups...";
                }
                else
                {
                    return ResourceGroup.ResourceGroupName;
                }
            }
        }

        public string SubDisplayText
        {
            get
            {
                if (IsPlaceholder)
                {
                    return "";
                }
                else
                {
                    return ResourceGroup.Location;
                }
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
