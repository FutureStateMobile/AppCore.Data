$appcmd = "$env:windir\system32\inetsrv\appcmd.exe"

<#
    .DESCRIPTION
        Creates an AppPool in IIS and sets up the specified identity to run under.

    .EXAMPLE
        New-AppPool

    .PARAMETER appPoolName
        The name of the application pool.

    .PARAMETER appPoolIdentityType
        The type of identity you want the AppPool to run as, default is 'LocalSystem'. 

    .PARAMETER maxProcesses
        The number of Worker Processes this AppPool should spawn, default is 1.

    .PARAMETER username
        The Username that this app pool should run as.

    .PARAMETER password
        The password for the Username that this app pool should run as.

    .SYNOPSIS
        Will setup an App Pool with a Managed Runtime Version of 4.0 and it defaults to using an Identity of LocalSystem.
#>
function New-AppPool{
    param(
        [parameter(Mandatory=$true, position=0)] [string] $appPoolName,
        [parameter(Mandatory=$false,position=1)] [ValidateSet("LocalSystem","LocalService","NetworkService","SpecificUser","ApplicationPoolIdentity")] [string] $appPoolIdentityType  = "NetworkService",
        [parameter(Mandatory=$false,position=2)] [int] $maxProcesses = 1,
        [parameter(Mandatory=$false,position=3)] [string] $username,
        [parameter(Mandatory=$false,position=4)] [string] $password,
        [parameter(Mandatory=$false,position=5)] [string] $managedPipelineMode = "Integrated",
        [parameter(Mandatory=$false,position=6)] [string] $managedRuntimeVersion = "v4.0"
    )

    Write-Output "Creating AppPool: $appPoolName"
    $exists = Confirm-AppPoolExists $appPoolName

    if (!$exists){
        $newAppPool = "$appcmd add APPPOOL"
        $newAppPool = "$newAppPool /name:$appPoolName"
        $newAppPool = "$newAppPool /processModel.identityType:$appPoolIdentityType"
        $newAppPool = "$newAppPool /processModel.maxProcesses:$maxProcesses"
        $newAppPool = "$newAppPool /managedPipelineMode:$managedPipelineMode"
        $newAppPool = "$newAppPool /managedRuntimeVersion:$managedRuntimeVersion"
        $newAppPool = "$newAppPool /autoStart:true"
    
        if ( $appPoolIdentityType -eq "SpecificUser" ){
            $newAppPool = "$newAppPool /processModel.userName:$username"
            $newAppPool = "$newAppPool /processModel.password:$password"
        }

        Invoke-Expression $newAppPool
        Write-Output "Created AppPool: $appPoolName"
    }else{
        Write-Output "AppPool already exits..."
        Update-AppPool $appPoolName $appPoolIdentityType $maxProcesses $username $password $managedPipelineMode $managedRuntimeVersion
    }
}

function Update-AppPool{
    param(
        [parameter(Mandatory=$true, position=0)] [string] $appPoolName,
        [parameter(Mandatory=$false,position=1)] [ValidateSet("LocalSystem","LocalService","NetworkService","SpecificUser","ApplicationPoolIdentity")] [string] $appPoolIdentityType = "NetworkService",
        [parameter(Mandatory=$false,position=2)] [int] $maxProcesses = 1,
        [parameter(Mandatory=$false,position=3)] [string] $username,
        [parameter(Mandatory=$false,position=4)] [string] $password,
        [parameter(Mandatory=$false,position=5)] [string] $managedPipelineMode = "Integrated",
        [parameter(Mandatory=$false,position=6)] [string] $managedRuntimeVersion = "v4.0"
    )

    Write-Output "Updating AppPool: $appPoolName"
    $exists = Confirm-AppPoolExists $appPoolName

    if ($exists){
        $updateAppPool = "$appcmd set APPPOOL $appPoolName"
        $updateAppPool = "$updateAppPool /processModel.identityType:$appPoolIdentityType"
        $updateAppPool = "$updateAppPool /processModel.maxProcesses:$maxProcesses"
        $updateAppPool = "$updateAppPool /managedPipelineMode:$managedPipelineMode"
        $updateAppPool = "$updateAppPool /managedRuntimeVersion:$managedRuntimeVersion"
        $updateAppPool = "$updateAppPool /autoStart:true"
    
        if ( $appPoolIdentityType -eq "SpecificUser" ){
            $updateAppPool = "$updateAppPool /processModel.userName:$username"
            $updateAppPool = "$updateAppPool /processModel.password:$password"
        }

        Invoke-Expression $updateAppPool
        Write-Output "Updated AppPool: $appPoolName"
    }else{
        Write-Output "Error: Could not find an AppPool with the name: $appPoolName"
    }
}

function Confirm-AppPoolExists( $appPoolName ){
    $getAppPool = Get-AppPool($appPoolName)
    return ($getAppPool -ne $null)
}

function Get-AppPool( $appPoolName ){
    $getAppPools = "$appcmd list APPPOOL $appPoolName"
    return Invoke-Expression $getAppPools
}

function Get-AppPools{
    $getAppPools = "$appcmd list APPPOOLS"
    Invoke-Expression $getAppPools
}

function Start-AppPool( $appPoolName ){
    $getAppPools = "$appcmd start APPPOOL $appPoolName"
    return Invoke-Expression $getAppPools
}

function Stop-AppPool( $appPoolName ){
    $getAppPools = "$appcmd stop APPPOOL $appPoolName"
    return Invoke-Expression $getAppPools
}

function Remove-AppPool( $appPoolName ){
    $getAppPools = "$appcmd delete APPPOOL $appPoolName"
    return Invoke-Expression $getAppPools
}

function Get-ModuleDirectory {
    return Split-Path $script:MyInvocation.MyCommand.Path
}

Export-ModuleMember New-AppPool
Export-ModuleMember Get-AppPool
Export-ModuleMember Get-AppPools
Export-ModuleMember Update-AppPool
Export-ModuleMember Remove-AppPool
Export-ModuleMember Start-AppPool
Export-ModuleMember Stop-AppPool
Export-ModuleMember Confirm-AppPoolExists