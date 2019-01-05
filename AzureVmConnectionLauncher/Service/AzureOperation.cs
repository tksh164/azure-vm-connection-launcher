using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using AzureVmConnectionLauncher.Model;

namespace AzureVmConnectionLauncher.Service
{
    internal static class AzureOperation
    {
        private static class AzureCli
        {
            private static string GetPythonExePath()
            {
                const string pythonExePath = @"%ProgramFiles(x86)%\Microsoft SDKs\Azure\CLI2\python.exe";
                var resolvedPythonExePath = Environment.ExpandEnvironmentVariables(pythonExePath);
                if (!File.Exists(resolvedPythonExePath))
                {
                    throw new FileNotFoundException("The python.exe does not exist.", resolvedPythonExePath);
                }
                return resolvedPythonExePath;
            }

            public static object InvokeAzureCliCommand(string cliCommand)
            {
                StringBuilder stdout = new StringBuilder();
                StringBuilder stderr = new StringBuilder();

                using (var proc = new Process())
                {
                    proc.StartInfo.FileName = GetPythonExePath();
                    proc.StartInfo.Arguments = string.Format(@"-IBm azure.cli {0}", cliCommand);

                    proc.StartInfo.LoadUserProfile = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;

                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.StandardOutputEncoding = Encoding.Default;
                    proc.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
                        stdout.Append(e.Data);
                    };

                    proc.StartInfo.RedirectStandardError = true;
                    proc.StartInfo.StandardErrorEncoding = Encoding.Default;
                    proc.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
                        stderr.Append(e.Data);
                    };

                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                    proc.WaitForExit();

                    if (proc.ExitCode != 0)
                    {
                        throw new Exception(string.Format("The Azure CLI command invocation failed. ExitCode={0}, CLICommand={1}", proc.ExitCode, cliCommand));
                    }
                }

                var cliCommandResult = stdout.ToString();
                return JsonConvert.DeserializeObject(cliCommandResult);
            }
        }

        public static ReadOnlyCollection<AzureSubscription> ConnectAccount()
        {
            dynamic cmdResults = AzureCli.InvokeAzureCliCommand("login --output json");

            var subscriptions = new List<AzureSubscription>();
            foreach (var sub in cmdResults)
            {
                var subscription = new AzureSubscription()
                {
                    SubscriptionId = sub.id,
                    TenantId = sub.tenantId,
                    Name = sub.name,
                    State = sub.state,
                };
                subscriptions.Add(subscription);
            }
            return subscriptions.AsReadOnly();
        }

        public static ReadOnlyCollection<AzureResourceGroup> GetResourceGroups(string subscriptionId)
        {
            dynamic cmdResults = AzureCli.InvokeAzureCliCommand(string.Format("group list --subscription {0} --output json", subscriptionId));

            var resourceGroups = new List<AzureResourceGroup>();
            foreach (var rg in cmdResults)
            {
                var resourceGroup = new AzureResourceGroup()
                {
                    ResourceGroupName = rg.name,
                    ResourceId = rg.id,
                    Location = rg.location,
                };
                resourceGroups.Add(resourceGroup);
            }
            return resourceGroups.AsReadOnly();
        }

        public static ReadOnlyCollection<AzureVirtualMachine> GetVirtualMachines(string subscriptionId, string resourceGroupName)
        {
            dynamic cmdResults = AzureCli.InvokeAzureCliCommand(string.Format("vm list --subscription {0} --resource-group {1} --show-details --output json", subscriptionId, resourceGroupName));

            var virtualmachines = new List<AzureVirtualMachine>();
            foreach (var vm in cmdResults)
            {
                var virtualMachine = new AzureVirtualMachine()
                {
                    VMName = vm.name,
                    AdminUserName = vm.osProfile.adminUsername,
                    OSType = vm.storageProfile.osDisk.osType,
                    PowerState = vm.powerState,
                    ResourceGroupName = vm.resourceGroup,
                };

                var connectionDestinations = new List<ConnectionDestination>();
                connectionDestinations.AddRange(GetConnectionDestinationsFromResultText((string)vm.fqdns, ConnectionDestinationType.FQDN));
                connectionDestinations.AddRange(GetConnectionDestinationsFromResultText((string)vm.publicIps, ConnectionDestinationType.PublicIPAddress));
                connectionDestinations.AddRange(GetConnectionDestinationsFromResultText((string)vm.privateIps, ConnectionDestinationType.PrivateIPAddress));
                virtualMachine.ConnectionDestinations = connectionDestinations.AsReadOnly();

                virtualmachines.Add(virtualMachine);
            }
            return virtualmachines.AsReadOnly();
        }

        private static ReadOnlyCollection<ConnectionDestination> GetConnectionDestinationsFromResultText(string textLine, ConnectionDestinationType connectionDestinationType)
        {
            var connectionDestinations = new List<ConnectionDestination>();
            foreach (var part in textLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                connectionDestinations.Add(new ConnectionDestination(part, connectionDestinationType));
            }
            return connectionDestinations.AsReadOnly();
        }
    }
}
