namespace AzureVmConnectionLauncher.Service
{
    public interface IVMConnectionService
    {
        void StartRdpProcess(string connectionDestination, string adminUserName);
        void StartSshProcess(string connectionDestination, string adminUserName);
    }

    public class VMConnectionService : IVMConnectionService
    {
        public void StartRdpProcess(string connectionDestination, string userName)
        {
            MstscRdpConnection.LaunchMstsc(connectionDestination, userName);
        }

        public void StartSshProcess(string connectionDestination, string userName)
        {
            OpenSshConnection.LaunchOpenSsh(connectionDestination, userName);
        }
    }
}
