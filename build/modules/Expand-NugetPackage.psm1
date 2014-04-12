<#
    .DESCRIPTION
        Extracts a Nuget Package file and puts the contents in the specified location.

    .EXAMPLE
        Expand-NugetPackage "something.pkg" "C:\temp\zipcontents"

    .PARAMETER nugetPackageName
        The full path to the Nuget package.

    .PARAMETER destinationFolder
         The full path to the desired destination.

    .SYNOPSIS
        Expands any Nuget package and places the contents in the specified folder.

    .NOTES
        Nothing yet...
#>
function Expand-NugetPackage
{
    param(
        [parameter(Mandatory=$true,position=0)] [string] $nugetPackageName,
        [parameter(Mandatory=$true,position=1)] [string] $destinationFolder
    )

    $ErrorActionPreference = "Stop"

    $pwd = Get-ModuleDirectory
    Import-Module $pwd\Expand-ZipFile

    Expand-ZipFile $nugetPackageName $destinationFolder

    Remove-Item "$destinationFolder\``[Content_Types``].xml"
    Remove-Item "$destinationFolder\*.nuspec"
    Remove-Item "$destinationFolder\_rels" -recurse
    Remove-Item "$destinationFolder\package" -recurse

    Import-Module $pwd\TextUtils
    Get-ChildItem $destinationFolder -recurse | where {$_.Mode -match "d"} | move-item -ea SilentlyContinue -dest { ( Invoke-UrlDecode $_.FullName ) }
    Get-ChildItem $destinationFolder -Recurse | Where-Object { !$_.PSIsContainer } |  Rename-Item -ea SilentlyContinue -NewName { Invoke-UrlDecode $_.Name }

}

function Get-ModuleDirectory {
    return Split-Path $script:MyInvocation.MyCommand.Path
}

Export-ModuleMember Expand-NugetPackage



