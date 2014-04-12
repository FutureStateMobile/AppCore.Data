<#
    .DESCRIPTION
        This will code sign the application with the specified certificate.

    .EXAMPLE
        Invoke-SignClickOnce "someapplication.manifest" "Somebiglonghashvaluethatmatchesthekeyneeded=="

    .PARAMETER LicenseKeyVerion
        The version of the software you are registering

    .PARAMETER LicenseKeyHash
        A hash of the license key which is to be stored in the registry.

    .SYNOPSIS
        Signs and application with the specified Cert Key Hash.

    .NOTES
        Nothing yet...
#>
function Invoke-SignClickOnce
{
    param( 
        [parameter(Mandatory=$true,position=0)] [string] $version,
        [parameter(Mandatory=$true,position=1)] [string] $keyHash
    )


}

function Get-ModuleDirectory {
    return Split-Path $script:MyInvocation.MyCommand.Path
}

Export-ModuleMember Invoke-SignClickOnce