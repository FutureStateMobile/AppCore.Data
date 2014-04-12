<#
    .DESCRIPTION
        Adds the specified line to the hosts file if it doesn't already exist.

    .EXAMPLE
        Add-HostsFileEntry "127.0.0.1" "local.marquisalliance.com"

    .PARAMETER ipAddress
        The ip address of the machine you want to target.

    .PARAMETER hostName
        The "C" name you want routed to the target ip address.

    .SYNOPSIS
        Will add a hosts file entry for the host name specified targeting the specified ip address.

    .NOTES
        Nothing yet...
#>
function Add-HostsFileEntry
{
	param(
		[parameter(Mandatory=$true,position=0)] [string] $ipAddress,
		[parameter(Mandatory=$true,position=1)]	[string] $hostName
	)

    $ErrorActionPreference = "Stop"

	Write-Host "Adding hosts entry for custom Host Header..."

	$HostsLocation = "$env:windir\System32\drivers\etc\hosts";
	$NewHostEntry = "`t$ipAddress`t$hostName";

	if((gc $HostsLocation) -contains $NewHostEntry)
	{
		Write-Host "The hosts file already contains the entry: $NewHostEntry.  File not updated.";
	}
	else
	{
		Write-Host "The hosts file does not contain the entry: $NewHostEntry.  Attempting to update.";
		Add-Content -Path $HostsLocation -Value $NewHostEntry;
	}

	# Validate entry
	if((gc $HostsLocation) -contains $NewHostEntry)
	{
		Write-Host "$NewHostEntry in $HostsLocation.";
	}
	else
	{
		Write-Host -Fore Red "Error $NewHostEntry is not in $HostsLocation.";
	}
}

Export-ModuleMember Add-HostsFileEntry