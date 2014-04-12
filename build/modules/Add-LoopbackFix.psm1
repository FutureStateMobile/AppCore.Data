<#
    .DESCRIPTION
        Adds a registry entry that overrides the default windows behaviour regarding allowing a different "C" name to point back to 127.0.0.1

    .EXAMPLE
        Add-LoopbackFix "local.marquisalliance.com"

    .PARAMETER siteHostName
        The hostname that you want to loop back to the local host.

    .SYNOPSIS
        Will apply a registry entry fix for issue KB896861.  Uses the more conservative and recommend fix found in method 1.

    .NOTES
        Applies method 1 workaround fix for "You receive error 401.1 when you browse a Web site that uses Integrated Authentication and is hosted on 
        IIS 5.1 or a later version"

    .LINK
        http://support.microsoft.com/kb/896861
#>
function Add-LoopbackFix
{
    param(
        [parameter(Mandatory=$true,position=0)] [string] $siteHostName
    )

    $ErrorActionPreference = "Stop"

    Write-Host "Creating fix for using custom host headers..."
    if (Get-ItemProperty -Name "BackConnectionHostNames" -path 'HKLM:\SYSTEM\CurrentControlSet\Control\Lsa\MSV1_0' -erroraction silentlycontinue) { 
        Write-Host "Fix for using custom host headers already in place."
    } else { 
        New-ItemProperty "HKLM:\SYSTEM\CurrentControlSet\Control\Lsa\MSV1_0" -Name "BackConnectionHostNames" -Value $siteHostName -PropertyType "MultiString" 
        Write-Host "Fix for using custom host headers added."
    }
}

Export-ModuleMember Add-LoopbackFix