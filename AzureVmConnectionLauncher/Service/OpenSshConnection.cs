using System;
using System.Diagnostics;

namespace AzureVmConnectionLauncher.Service
{
    public static class OpenSshConnection
    {
        public static void LaunchOpenSsh(string server, string userName)
        {
            StartOpenSshProcess(server, userName);
        }

        private static bool StartOpenSshProcess(string server, string userName)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%windir%\System32\OpenSSH\ssh.exe");
                process.StartInfo.Arguments = string.Format(@"{0}@{1}", userName, server);
                process.StartInfo.LoadUserProfile = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                return process.Start();
            }
        }
    }
}
