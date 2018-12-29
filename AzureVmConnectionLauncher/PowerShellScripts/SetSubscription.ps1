param (
	[Parameter(Mandatory = $true)]
	[string] $SubscriptionId
)

$result = Set-AzureRmContext -Subscription $SubscriptionId
$result.Subscription
