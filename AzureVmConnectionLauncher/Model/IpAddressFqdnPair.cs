namespace AzureVmConnectionLauncher.Model
{
    public class IpAddressFqdnPair
    {
        public IpAddressFqdnPair(string ipAddress, string fqdn)
        {
            IpAddress = ipAddress;
            Fqdn = fqdn;
        }

        public string IpAddress { get; private set; }
        public string Fqdn { get; private set; }
    }
}
