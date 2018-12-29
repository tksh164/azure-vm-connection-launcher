using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using AzureVmConnectionLauncher.Model;

namespace AzureVmConnectionLauncher.ViewModel
{
    public class AzureVirtualMachineViewModel : ViewModelBase
    {
        public AzureVirtualMachine VirtualMachine { get; set; }

        private ObservableCollection<IpAddressFqdnPairViewModel> ipAddressFqdnPairs;
        public ObservableCollection<IpAddressFqdnPairViewModel> IpAddressFqdnPairs
        {
            get
            {
                return ipAddressFqdnPairs;
            }
            set
            {
                ipAddressFqdnPairs = value;
                RaisePropertyChanged(nameof(IpAddressFqdnPair));
            }
        }

        public bool IsPlaceholder { get; set; }

        public string DisplayText
        {
            get
            {
                if (IsPlaceholder)
                {
                    return "Loading virtual machines...";
                }
                else
                {
                    return string.Format("{0} ({1}, {2})", VirtualMachine.VMName, VirtualMachine.OSType, VirtualMachine.PowerState);
                }
            }
        }
    }
}
