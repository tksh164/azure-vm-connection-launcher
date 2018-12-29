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
        public ReadOnlyCollection<IpAddressFqdnPair> IpAddressFqdnPairs { get; set; }
    }
}
