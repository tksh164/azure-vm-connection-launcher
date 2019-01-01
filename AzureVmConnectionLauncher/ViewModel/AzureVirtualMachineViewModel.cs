using System.Collections.ObjectModel;
using System.Windows;
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

        public string MainDisplayText
        {
            get
            {
                if (IsPlaceholder)
                {
                    return "Loading virtual machines...";
                }
                else
                {
                    return VirtualMachine.VMName;
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
                    return string.Format("{0}, {1}", VirtualMachine.PowerState, VirtualMachine.OSType);
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
