using System.Collections.ObjectModel;

namespace AzureVmConnectionLauncher.Model
{
    public class AzureVirtualMachine
    {
        public string VMName { get; set; }
        public string AdminUserName { get; set; }
        public string OSType { get; set; }
        public string PowerState { get; set; }
        public string ResourceGroupName { get; set; }
        public ReadOnlyCollection<ConnectionDestination> ConnectionDestinations { get; set; }

        //vm.id
        //vm.location
        //vm.vmId
        //vm.hardwareProfile.vmSize
        //vm.osProfile.computerName
        //vm.osProfile.windowsConfiguration
        //vm.osProfile.linuxConfiguration
        //vm.storageProfile.imageReference.offer
        //vm.storageProfile.imageReference.publisher
        //vm.storageProfile.imageReference.sku
    }
}
