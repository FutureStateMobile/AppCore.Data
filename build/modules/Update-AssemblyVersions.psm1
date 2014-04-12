<#
    .DESCRIPTION
        This will search for AssemblyInfo.cs files in the current directory and its sub directories, 
        and then updates the AssemblyVersion and AssemblyFileVersion.

    .EXAMPLE
        Update-AssemblyVersions '3.1.2', '233', '3.1.2 Beta 1'

    .PARAMETER Version
        A string containing the version of this dll.  This would be in the format of {Major}.{Minor}.{Revision}

    .PARAMETER BuildNumber
        An optional object (of any type) to be passed in to the scriptblock (available as $input)

    .PARAMETER AssemblyInformationalVersion
        A switch that enables powershell profile loading for the elevated command/block

    .SYNOPSIS
        Update all the AssemblyInfo.cs files in a solution so they are the same.

    .NOTES
        Nothing yet...
#> 
function Update-AssemblyVersions
{
    param( 
        [parameter(Mandatory=$true,position=0)] [string] $Version,
        [parameter(Mandatory=$true,position=1)] [string] $BuildNumber,
        [parameter(Mandatory=$true,position=2)] [string] $AssemblyInformationalVersion
    )

    $ErrorActionPreference = "Stop"

    $assemblyVersionPattern = 'AssemblyVersion\(".*?"\)'
    $fileVersionPattern = 'AssemblyFileVersion\(".*?"\)'
    $informationalVersionPattern = 'AssemblyInformationalVersion\(".*?"\)'
    
    $assemblyVersion = 'AssemblyVersion("' + $Version + '")';
    $fileVersion = 'AssemblyFileVersion("' + $Version + '.' + $BuildNumber + '")';
    $informationalVersion = 'AssemblyInformationalVersion("' + $AssemblyInformationalVersion + '")';
    
    Write-Host "Updating AssemblyVersion to $Version"
    Write-Host "Updating AssemblyFileVersion to $Version.$BuildNumber"
    Write-Host "Updating AssemblyInformationalVersion to $AssemblyInformationalVersion"

    Set-Location ..\

    Get-ChildItem -r -filter AssemblyInfo.cs | ForEach-Object {
        $filename = $_.Directory.ToString() + '\' + $_.Name
        
        # If you are using a source control that requires to check-out files before 
        # modifying them, make sure to check-out the file here.
        # For example, TFS will require the following command:
        # tf checkout $filename
    
        (Get-Content $filename) | ForEach-Object {
            % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
            % {$_ -replace $fileVersionPattern, $fileVersion } |
            % {$_ -replace $informationalVersionPattern, $informationalVersion }
        } | Set-Content $filename

        Write-Host "$filename - Updated"
    }
}

Export-ModuleMember Update-AssemblyVersions