param (
    [Parameter(Mandatory = $true)]
    [string] $ResourceGroupName
)

function Get-VMOSType
{
    param (
        [Parameter(Mandatory = $true)][ValidateNotNull()]
        [Microsoft.Azure.Commands.Compute.Models.PSVirtualMachine] $VM
    )

    if ($VM.OSProfile.WindowsConfiguration -ne $null)
    {
        return 'windows'
    }
    elseif ($VM.OSProfile.LinuxConfiguration -ne $null)
    {
        return 'linux'
    }

    return 'unknown'
}

function Get-VMPowerState
{
    param (
        [Parameter(Mandatory = $true)][ValidateNotNull()]
        [Microsoft.Azure.Commands.Compute.Models.PSVirtualMachine] $VM
    )

    if ($VM.PowerState -eq 'VM running')
    {
        return 'running'
    }
    elseif ($VM.PowerState -eq 'VM deallocated')
    {
        return 'deallocated'
    }

    return 'unknown'
}

function Get-ResourceIdParts
{
    param (
        [Parameter(Mandatory = $true)]
        [string] $ResourceId
    )

    $parts = $netInterfaceId.Split('/')
    return @(
        $parts[2],  # Subscription ID
        $parts[4],  # Resource group name
        $parts[6],  # Resource provider name
        $parts[7],  # Resource type name without resource provider name
        $parts[8]   # Resource name
    )
}

function Get-IpAddressFromPublicIpAddress
{
    param (
        [Parameter(Mandatory = $true)]
        [Microsoft.Azure.Commands.Network.Models.PSPublicIpAddress] $PublicIpAddress
    )

    if (($PublicIpAddress.PublicIpAllocationMethod -eq 'Static'))
    {
        return $PublicIpAddress.IpAddress
    }
    elseif (($PublicIpAddress.PublicIpAllocationMethod -eq 'Dynamic') -and ($PublicIpAddress.IpAddress -ne 'Not Assigned'))
    {
        return $PublicIpAddress.IpAddress
    }

    return $null
}

function Get-FqdnFromPublicIpAddress
{
    param (
        [Parameter(Mandatory = $true)]
        [Microsoft.Azure.Commands.Network.Models.PSPublicIpAddress] $PublicIpAddress
    )

    if ($PublicIpAddress.DnsSettings -ne $null)
    {
        return $PublicIpAddress.DnsSettings.Fqdn
    }

    return $null
}

#Import-Module -Name 'AzureRM.Network' -Force
#Import-Module -Name 'AzureRM.Compute' -Force

# Get the all public IP addresses in the subscription.
$publicIpAddresses = Get-AzureRmPublicIpAddress

# Create a dictionary of the public IP addresses by resource ID for finding the public IP address.
$publicIpAddressDictionary = @{}
foreach ($publicIpAddress in $publicIpAddresses)
{
    $publicIpAddressDictionary[$publicIpAddress.Id] = $publicIpAddress
}

# Get the all virtual machines in the specified resource group.
$vms = Get-AzureRmVM -Status -ResourceGroup $ResourceGroupName

foreach ($vm in $vms)
{
    $vmConnectionInfo = [pscustomobject] @{
        ResourceGroupName = $vm.ResourceGroupName
        VMName            = $vm.Name
        OSType            = Get-VMOSType -VM $vm
        PowerState        = Get-VMPowerState -VM $vm
        AdminUserName     = $vm.OSProfile.AdminUsername
        IpAddressFqdnPairs = @()
    }

    # Get the public IP addresses and FQDNs from each IP configurations on each network interfaces.
    $netInterfaceIds = $vm.NetworkProfile.NetworkInterfaces.Id
    foreach ($netInterfaceId in $netInterfaceIds)
    {
        ($null, $netInterfaceResourceGroupName, $null, $null, $netInterfaceName) = Get-ResourceIdParts -ResourceId $netInterfaceId
        $netInterface = Get-AzureRmNetworkInterface -ResourceGroupName $netInterfaceResourceGroupName -Name $netInterfaceName

        foreach ($ipConfig in $netInterface.IpConfigurations)
        {
            $publicIpAddress = $publicIpAddressDictionary[$ipConfig.PublicIpAddress.Id]

            $ipAddress = Get-IpAddressFromPublicIpAddress -PublicIpAddress $publicIpAddress
            $fqdn      = Get-FqdnFromPublicIpAddress -PublicIpAddress $publicIpAddress

            if ($ipAddress -ne $null)
            {
                $vmConnectionInfo.IpAddressFqdnPairs += [pscustomobject] @{
                    IpAddress = $ipAddress
                    Fqdn      = $fqdn
                }
            }
        }
    }

    $vmConnectionInfo
}
