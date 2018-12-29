using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureVmConnectionLauncher.Service
{
    internal static class MstscRdpConnection
    {
        public static void LaunchMstsc(string server, string userName)
        {
            var tempRdpFilePath = CreateTempRdpFile(server, userName);
            StartMstscProcess(tempRdpFilePath);
            Task.Run(() => {
                Thread.Sleep(1000);
                File.Delete(tempRdpFilePath);
            });
            Debug.WriteLine(string.Format("RDP File Path: {0}", tempRdpFilePath));
        }

        private static string CreateTempRdpFile(string server, string userName)
        {
            var tempRdpFilePath = Path.GetTempFileName();
            var rdpFileContent = GetRdpFileContent(server, userName);
            File.WriteAllText(tempRdpFilePath, rdpFileContent, Encoding.UTF8);
            return tempRdpFilePath;
        }

        private static string GetRdpFileContent(string server, string userName)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Format(@"full address:s:{0}:3389", server));
            builder.AppendLine(string.Format(@"username:s:{0}", userName));
            builder.AppendLine(@"prompt for credentials:i:1");
            builder.AppendLine(@"administrative session:i:1");
            return builder.ToString();
        }

        private static bool StartMstscProcess(string rdpFilePath)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%windir%\system32\mstsc.exe");
                process.StartInfo.Arguments = @"""" + rdpFilePath + @"""";
                process.StartInfo.LoadUserProfile = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                return process.Start();
            }
        }
    }
}
