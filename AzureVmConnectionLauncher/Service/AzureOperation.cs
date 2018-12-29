using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using AzureVmConnectionLauncher.Model;

namespace AzureVmConnectionLauncher.Service
{
    internal static class AzureOperation
    {
        public static void ConnectAccount()
        {
            using (var runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();

                using (var psInstance = PowerShell.Create())
                {
                    psInstance.Runspace = runspace;

                    const string scriptFilePath = @".\PowerShellScripts\ConnectAzure.ps1";
                    var scriptContent = File.ReadAllText(scriptFilePath);
                    psInstance.AddScript(scriptContent);

                    var psOutputs = psInstance.Invoke();

                    //var result = psInstance.BeginInvoke();
                    //result.AsyncWaitHandle.WaitOne();
                    //var psOutputs = psInstance.EndInvoke(result);
                    //result.AsyncWaitHandle.Close();

                    //var account = new AzureAccount();
                    //foreach (var outputItem in psOutputs)
                    //{
                    //    if (outputItem != null)
                    //    {
                    //        var v = outputItem.Properties["Environments"].Value;
                    //        //Console.WriteLine();
                    //    }
                    //}
                    //return account;
                }
            }
        }

        public static ReadOnlyCollection<AzureSubscription> GetSubscriptions()
        {
            using (var runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();

                using (var psInstance = PowerShell.Create())
                {
                    psInstance.Runspace = runspace;

                    const string scriptFilePath = @".\PowerShellScripts\GetSubscriptions.ps1";
                    var scriptContent = File.ReadAllText(scriptFilePath);
                    psInstance.AddScript(scriptContent);

                    var psOutputs = psInstance.Invoke();
                    var subscriptions = new List<AzureSubscription>();
                    foreach (var outputItem in psOutputs)
                    {
                        if (outputItem != null)
                        {
                            var subscription = new AzureSubscription()
                            {
                                SubscriptionId = (string)outputItem.Properties["SubscriptionId"].Value,
                                TenantId = (string)outputItem.Properties["TenantId"].Value,
                                Name = (string)outputItem.Properties["Name"].Value,
                                State = (string)outputItem.Properties["State"].Value,
                            };
                            subscriptions.Add(subscription);
                        }
                    }
                    return subscriptions.AsReadOnly();
                }
            }
        }

        public static AzureSubscription SetCurrentSubscription(string subscriptionId)
        {
            using (var runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();

                using (var psInstance = PowerShell.Create())
                {
                    psInstance.Runspace = runspace;

                    const string scriptFilePath = @".\PowerShellScripts\SetSubscription.ps1";
                    var scriptContent = File.ReadAllText(scriptFilePath);
                    psInstance.AddScript(scriptContent);
                    psInstance.AddParameter("SubscriptionId", subscriptionId);

                    var psOutputs = psInstance.Invoke();
                    foreach (var outputItem in psOutputs)
                    {
                        if (outputItem != null)
                        {
                            var subscription = new AzureSubscription()
                            {
                                SubscriptionId = (string)outputItem.Properties["SubscriptionId"].Value,
                                TenantId = (string)outputItem.Properties["TenantId"].Value,
                                Name = (string)outputItem.Properties["Name"].Value,
                                State = (string)outputItem.Properties["State"].Value,
                            };
                            return subscription;
                        }
                    }

                    throw new InvalidDataException();
                }
            }
        }

        public static ReadOnlyCollection<AzureResourceGroup> GetResourceGroups()
        {
            using (var runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();

                using (var psInstance = PowerShell.Create())
                {
                    psInstance.Runspace = runspace;

                    const string scriptFilePath = @".\PowerShellScripts\GetResourceGroups.ps1";
                    var scriptContent = File.ReadAllText(scriptFilePath);
                    psInstance.AddScript(scriptContent);

                    var psOutputs = psInstance.Invoke();
                    var resourceGroups = new List<AzureResourceGroup>();
                    foreach (var outputItem in psOutputs)
                    {
                        var resourceGroup = new AzureResourceGroup()
                        {
                            ResourceGroupName = (string)outputItem.Properties["ResourceGroupName"].Value,
                            ResourceId = (string)outputItem.Properties["ResourceId"].Value,
                            Location = (string)outputItem.Properties["Location"].Value,
                        };
                        resourceGroups.Add(resourceGroup);
                    }
                    return resourceGroups.AsReadOnly();
                }
            }
        }

        public static ReadOnlyCollection<AzureVirtualMachine> GetVirtualMachines(string resourceGroupName)
        {
            using (var runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();

                using (var psInstance = PowerShell.Create())
                {
                    psInstance.Runspace = runspace;

                    const string scriptFilePath = @".\PowerShellScripts\GetVMs.ps1";
                    var scriptContent = File.ReadAllText(scriptFilePath);
                    psInstance.AddScript(scriptContent);
                    psInstance.AddParameter("ResourceGroupName", resourceGroupName);

                    var psOutputs = psInstance.Invoke();
                    var virtualmachines = new List<AzureVirtualMachine>();
                    foreach (var outputItem in psOutputs)
                    {
                        if (outputItem != null)
                        {
                            var virtualMachine = new AzureVirtualMachine()
                            {
                                VMName = (string)outputItem.Properties["VMName"].Value,
                                AdminUserName = (string)outputItem.Properties["AdminUserName"].Value,
                                OSType = (string)outputItem.Properties["OSType"].Value,
                                PowerState = (string)outputItem.Properties["PowerState"].Value,
                                ResourceGroupName = (string)outputItem.Properties["ResourceGroupName"].Value,
                            };

                            var ipAddressFqdnPairs = new List<IpAddressFqdnPair>();
                            foreach (PSObject pair in (object[])outputItem.Properties["IpAddressFqdnPairs"].Value)
                            {
                                var ipAddress = (string)pair.Properties["IpAddress"].Value;
                                var fqdn = (string)pair.Properties["Fqdn"].Value;

                                var ipAddressFqdnPair = new IpAddressFqdnPair(ipAddress, fqdn);
                                ipAddressFqdnPairs.Add(ipAddressFqdnPair);
                            }
                            virtualMachine.IpAddressFqdnPairs = ipAddressFqdnPairs.AsReadOnly();

                            virtualmachines.Add(virtualMachine);
                        }
                    }
                    return virtualmachines.AsReadOnly();
                }
            }
        }
    }
}
