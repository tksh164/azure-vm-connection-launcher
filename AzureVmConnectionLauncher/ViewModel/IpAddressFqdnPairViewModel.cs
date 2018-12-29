using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using AzureVmConnectionLauncher.Model;
using AzureVmConnectionLauncher.Service;

namespace AzureVmConnectionLauncher.ViewModel
{
    public class IpAddressFqdnPairViewModel : ViewModelBase
    {
        public IpAddressFqdnPairViewModel()
        {
            ConnectVMUsingRdpCommand = new RelayCommand(ConnectVMUsingRdp);
            ConnectVMUsingSshCommand = new RelayCommand(ConnectVMUsingSsh);
        }

        public AzureVirtualMachine VirtualMachine { get; set; }

        public IpAddressFqdnPair IpAddressFqdnPair { get; set; }

        public RelayCommand ConnectVMUsingRdpCommand { get; set; }

        private void ConnectVMUsingRdp()
        {
            var vmConnectionService = new VMConnectionService();
            vmConnectionService.StartRdpProcess(IpAddressFqdnPair.IpAddress, IpAddressFqdnPair.Fqdn, VirtualMachine.AdminUserName);
        }

        public RelayCommand ConnectVMUsingSshCommand { get; set; }

        private void ConnectVMUsingSsh()
        {
            var vmConnectionService = new VMConnectionService();
            vmConnectionService.StartSshProcess(IpAddressFqdnPair.IpAddress, IpAddressFqdnPair.Fqdn, VirtualMachine.AdminUserName);
        }

        public string DisplayText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(IpAddressFqdnPair.Fqdn))
                {
                    return IpAddressFqdnPair.IpAddress;
                }
                else
                {
                    return string.Format("{0} ({1})", IpAddressFqdnPair.Fqdn, IpAddressFqdnPair.IpAddress);
                }
            }
        }
    }
}
