<#
    .DESCRIPTION
        Will deploy a new, or update an existing WebApplication using a specific settings file format.

    .EXAMPLE
        Deploy-WebApplication $configs

    .PARAMETER configs
        The applications configuration

    .PARAMETER Version
        The applications configuration

    .SYNOPSIS
        Will deploy a web application using the customized settings file.
#>

Import-Module WebAdministration

function Deploy-WebApplication
{
    param(
        [parameter( Mandatory=$true, position=0 )] $configs,
        [parameter( Mandatory=$true, position=1 )] [string] $Version
    )

    $ErrorActionPreference = "Stop"

    Write-Output "Deploying Web Application: $appName"
}

Export-ModuleMember Deploy-WebApplication