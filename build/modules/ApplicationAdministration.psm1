$appcmd = "$env:windir\system32\inetsrv\appcmd.exe"

<#
    .DESCRIPTION
        Will setup a web application under the specified Website and AppPool.

    .EXAMPLE
        New-Application "Mudtrack" "apps.marquisalliance.com" "C:\inetpub\apps.marquisalliance.com\mudtrack" "Mudtrack"

    .PARAMETER appName
        The name of the application.

    .PARAMETER appPath
        The physical path where this application is located on disk.

    .PARAMETER siteName
        The name of the website that contains this application.

    .PARAMETER appPoolName
        The application pool that this application runs under.

    .PARAMETER updateIfFound
        With this switch passed in, the Applications PhysicalPath will be updated to point to the new AppPath provided, otherwise, if it already exists it will just be left alone.

    .SYNOPSIS
        Will setup a web application under the specified Website and AppPool.
#>

Import-Module WebAdministration

function New-Application
{
    param(
        [parameter( Mandatory=$true, position=0 )] [string] $siteName,
        [parameter( Mandatory=$true, position=1 )] [string] $appName,
        [parameter( Mandatory=$true, position=2 )] [string] $appPath,
        [parameter( Mandatory=$true, position=3 )] [string] $appPoolName,
        [parameter( Mandatory=$false, position=4 )] [switch] $updateIfFound
    )

    $ErrorActionPreference = "Stop"

    Write-Output "Creating new Application: $siteName/$appName"
    $exists = Confirm-ApplicationExists $siteName $appName
    
    if (!$exists) {
        & $appcmd add App /site.name:$siteName /path:/$appName /physicalPath:$appPath
        & $appcmd set App /app.name:$siteName/$appName /applicationPool:$appPoolName
        Write-Output "Created Application: $siteName/$appName"
    } else {
        Write-Output "Application already exists..."
        if ($updateIfFound.isPresent) {
            Update-Application $siteName $appName $appPath $appPoolName
        } else {
            Write-Output "Not updating Application, you must specify the '-updateIfFound' if you wish to update the Application settings."
        }
    }
}

function Update-Application{
    param(
        [parameter( Mandatory=$true, position=0 )] [string] $siteName,
        [parameter( Mandatory=$true, position=1 )] [string] $appName,
        [parameter( Mandatory=$true, position=2 )] [string] $appPath,
        [parameter( Mandatory=$true, position=3 )] [string] $appPoolName
    )

    Write-Output "Updating Application: $siteName/$appName"
    $exists = Confirm-ApplicationExists $siteName $appName

    if ($exists){
        & $appcmd set App /app.name:$siteName/$appName /applicationPool:$appPoolName
        & $appcmd set app /app.name:$siteName/$appName "/[path='/'].physicalPath:$appPath"
        Write-Output "Updated Application: $siteName/$appName"
    }else{
        Write-Output "Error: Could not find an Application with the name: $siteName/$appName"
    }
}

function Confirm-ApplicationExists( $siteName, $appName ){
    $getApp = Get-Application $siteName $appName
    
    if ($getApp -ne $null){
        return $getApp.Contains( "APP ""$siteName/$appName")
    }

    return ($getApp -ne $null)
}

function Remove-Application( $siteName, $appName ){
    $getApp = "$appcmd delete App '$siteName/$appName/'"
    return Invoke-Expression $getApp
}

function Start-Application( $siteName, $appName ){
    $getApp = "$appcmd start App '$siteName/$appName/'"
    return Invoke-Expression $getApp
}

function Stop-Application( $siteName, $appName ){
    $getApp = "$appcmd stop App '$siteName/$appName/'"
    return Invoke-Expression $getApp
}

function Get-Application( $siteName, $appName ){
    $getApp = "$appcmd list App '$siteName/$appName/'"
    return Invoke-Expression $getApp
}

function Get-Applications{
    $getApp = "$appcmd list Apps"
    Invoke-Expression $getApp
}

Export-ModuleMember New-Application
Export-ModuleMember Update-Application
Export-ModuleMember Confirm-ApplicationExists
Export-ModuleMember Remove-Application
Export-ModuleMember Start-Application
Export-ModuleMember Stop-Application
Export-ModuleMember Get-Application
Export-ModuleMember Get-Applications
