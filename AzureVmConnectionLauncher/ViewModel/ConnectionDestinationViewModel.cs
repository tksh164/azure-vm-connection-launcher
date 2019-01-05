using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using AzureVmConnectionLauncher.Model;
using AzureVmConnectionLauncher.Service;

namespace AzureVmConnectionLauncher.ViewModel
{
    public class ConnectionDestinationViewModel : ViewModelBase
    {
        public ConnectionDestinationViewModel()
        {
            ConnectVMUsingRdpCommand = new RelayCommand(ConnectVMUsingRdp);
            ConnectVMUsingSshCommand = new RelayCommand(ConnectVMUsingSsh);
        }

        public AzureVirtualMachine VirtualMachine { get; set; }

        public ConnectionDestination ConnectionDestination { get; set; }

        public RelayCommand ConnectVMUsingRdpCommand { get; private set; }

        private void ConnectVMUsingRdp()
        {
            var vmConnectionService = new VMConnectionService();
            vmConnectionService.StartRdpProcess(ConnectionDestination.Destination, VirtualMachine.AdminUserName);
        }

        public RelayCommand ConnectVMUsingSshCommand { get; private set; }

        private void ConnectVMUsingSsh()
        {
            var vmConnectionService = new VMConnectionService();
            vmConnectionService.StartSshProcess(ConnectionDestination.Destination, VirtualMachine.AdminUserName);
        }

        public string DisplayText
        {
            get
            {
                return string.Format("{0} ({1})", ConnectionDestination.Destination, ConnectionDestination.Type);
            }
        }
    }
}
