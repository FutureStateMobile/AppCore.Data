<#
    .DESCRIPTION
       Will set the specified Authentication value for the specified applicaiton or website

    .EXAMPLE
        Set-IISAuthentication "windowsAuthentication" true "apps.marquisalliance.com/mudtrack"

    .PARAMETER settingName
        The name of the Authentication setting that we are changing

    .PARAMETER value
        What we want to change the setting to.

    .PARAMETER location
        The IIS location of the Application or Website that we want to change the setting on.

    .SYNOPSIS
        Will set the specified Authentication value for the specified applicaiton or website.
#>

Import-Module WebAdministration

function Set-IISAuthentication
{
    param(
        [parameter(Mandatory=$true,position=0)] [string] $settingName,
        [parameter(Mandatory=$true,position=1)] [PSObject] $value,
        [parameter(Mandatory=$true,position=2)] [string] $location
    )

    $ErrorActionPreference = "Stop"

    Write-Output "Setting $settingName to a value of $value."
    Set-WebConfigurationProperty -filter "/system.webServer/security/authentication/$settingName" -name enabled -value $value -PSPath "IIS:\" -location $location

}

Export-ModuleMember Set-IISAuthentication