namespace AzureVmConnectionLauncher.Model
{
    public enum ConnectionDestinationType
    {
        FQDN,
        PublicIPAddress,
        PrivateIPAddress,
    }

    public class ConnectionDestination
    {
        public ConnectionDestination(string destination, ConnectionDestinationType type)
        {
            Destination = destination;
            Type = type;
        }

        public string Destination { get; private set; }
        public ConnectionDestinationType Type { get; private set; }
    }
}
