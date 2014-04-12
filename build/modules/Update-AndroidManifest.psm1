<#
    .DESCRIPTION
        This will search for AndroidManifest.xml files in the current directory and its sub directories, 
        and then updates the versionCode and versionName attributes.

    .EXAMPLE
        Update-AndroidManifest '3.1.2', '233'

    .PARAMETER Version
        A string containing the version of this dll.  This would be in the format of {Major}.{Minor}.{Revision}

    .PARAMETER BuildNumber
        A number containing the current running build number for this project.

    .SYNOPSIS
        Updates all the AndroidManifest.xml file to the correct versionCode and versionName, it uses the BuildNumber as the versionCode,
        in the form of {BuildNumber} and uses the Version plus BuildNumber for the versionName in the form of {Major}.{Minor}.{Revision}.{BuildNumber}.

    .NOTES
        Nothing yet...
#> 
function Update-AndroidManifest
{
    param( 
        [parameter(Mandatory=$true,position=0)] [string] $Version,
        [parameter(Mandatory=$true,position=1)] [string] $BuildNumber
    )

    $ErrorActionPreference = "Stop"

    $versionCodePattern = 'versionCode=".*?"'
    $versionNamePattern = 'versionName=".*?"'
    
    $versionCode = 'versionCode="' + $BuildNumber + '"';
    $versionName = 'versionName="' + $Version + '.' + $BuildNumber + '"';

    Write-Host "Updating AndroidManifest versionCode to $BuildNumber"
    Write-Host "Updating AndroidManifest versionName to $Version.$BuildNumber"

    Set-Location ..\

    Get-ChildItem -r -filter AndroidManifest.xml | ForEach-Object {
        $filename = $_.Directory.ToString() + '\' + $_.Name
        
        # If you are using a source control that requires to check-out files before 
        # modifying them, make sure to check-out the file here.
        # For example, TFS will require the following command:
        # tf checkout $filename
    
        (Get-Content $filename) | ForEach-Object {
            % {$_ -replace $versionCodePattern, $versionCode } |
            % {$_ -replace $versionNamePattern, $versionName }
        } | Set-Content $filename

        Write-Host "$filename - Updated"
    }
}

Export-ModuleMember Update-AndroidManifest