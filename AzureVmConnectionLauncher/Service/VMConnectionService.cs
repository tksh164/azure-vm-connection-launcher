namespace AzureVmConnectionLauncher.Service
{
    public interface IVMConnectionService
    {
        void StartRdpProcess(string ipAddress, string fqdn, string adminUserName);
        void StartSshProcess(string ipAddress, string fqdn, string adminUserName);
    }

    public class VMConnectionService : IVMConnectionService
    {
        public void StartRdpProcess(string ipAddress, string fqdn, string userName)
        {
            var server = string.IsNullOrWhiteSpace(fqdn) ? ipAddress : fqdn;
            MstscRdpConnection.LaunchMstsc(server, userName);
        }

        public void StartSshProcess(string ipAddress, string fqdn, string userName)
        {
            var server = string.IsNullOrWhiteSpace(fqdn) ? ipAddress : fqdn;
            OpenSshConnection.LaunchOpenSsh(server, userName);
        }
    }
}
