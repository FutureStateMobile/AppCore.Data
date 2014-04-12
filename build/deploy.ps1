param( 
    [string] $Version = $OctopusPackageVersion,
    [string] $Environment = $OctopusEnvironmentName
)

$ErrorActionPreference = "Stop"

Import-Module WebAdministration

$currentDir = Split-Path $script:MyInvocation.MyCommand.Path

Import-Module $currentDir\modules\Get-EnvironmentSettings

$environmentSettings = Get-EnvironmentSettings $Environment "//environmentSettings"
$appFilePath = "$($environmentSettings.filePathRoot)\$($environmentSettings.siteHost)\$($environmentSettings.appPath)\$($environmentSettings.appName)-$Version"

if ($OctopusParameters) {
    if ($OctopusParameters["appPool.password"]){
        $environmentSettings.appPool.password = $OctopusParameters["appPool.password"]
    }
}

Import-Module $currentDir\modules\Format-TaskNameToHost

Format-TaskNameToHost "Configure Web.config"
$databaseSettings = $environmentSettings.serverDatabase
$connectionStringXPath = "//connectionStrings/add[@name='databaseConnection']"
$databaseNameStringXPath = "//appSettings/add[@key='databaseName']"

Import-Module $currentDir\modules\Update-XmlConfigFile

Update-ConfigValues $currentDir\website\Web.config $databaseNameStringXPath $($databaseSettings.name) "value"
Update-ConfigValues $currentDir\website\Web.config $connectionStringXPath $($databaseSettings.connectionString) "connectionString"
Update-ConfigValues $currentDir\website\Web.config $connectionStringXPath $($databaseSettings.providerName) "providerName"

Format-TaskNameToHost "Copy Folder Contents"
# Create Folder for the application
if(!(Test-Path $($appFilePath))) {
    md $($appFilePath)
} else {
    Remove-Item "$($appFilePath)\*" -recurse -Force
}

Write-Host "Copying website content to: $($appFilePath)"
Copy-Item "$currentDir\website\*" $($appFilePath) -Recurse -Force
Write-Host "Successfully copied website content"

Format-TaskNameToHost "Set Folder Security"
Write-Host "Granting read/write access to $($environmentSettings.filePathRoot)\$($environmentSettings.siteHost)"
icacls "$($environmentSettings.filePathRoot)\$($environmentSettings.siteHost)" /grant ($($config.appPool.userName) + ":(OI)(CI)(M)") | Out-Default
Write-Host "Successfully granted read/write access to $($environmentSettings.filePathRoot)\$($environmentSettings.siteHost)"

Write-Host ""
Write-Host "Granting read/write access to $($appFilePath)"
icacls "$($appFilePath)" /grant ($($config.appPool.userName) + ":(OI)(CI)(M)") | Out-Default
Write-Host "Successfully granted read/write access to $($appFilePath)"

Import-Module "$currentDir\modules\AppPoolAdministration"
Format-TaskNameToHost "Create AppPool"
New-AppPool $($environmentSettings.appPool.name) $($environmentSettings.appPool.identityType) $($environmentSettings.maxWorkerProcesses) $($environmentSettings.appPool.userName) $($environmentSettings.appPool.password)

Import-Module "$currentDir\modules\SiteAdministration"
Format-TaskNameToHost "Create Web Site"
New-Site $($environmentSettings.siteHost) "$($environmentSettings.filePathRoot)\$($environmentSettings.siteHost)" $($environmentSettings.siteHost) $($environmentSettings.appPool.name) -updateIfFound

Import-Module "$currentDir\modules\ApplicationAdministration"

Format-TaskNameToHost "Create Application"
New-Application $($environmentSettings.siteHost) $($environmentSettings.appPath) $appFilePath $($environmentSettings.appPool.name) -updateIfFound

Import-Module "$currentDir\modules\Set-IISAuthentication"

Format-TaskNameToHost "Setting IIS Authentication for $($environmentSettings.siteHost)"
Set-IISAuthentication windowsAuthentication false $($environmentSettings.siteHost)
Set-IISAuthentication anonymousAuthentication true $($environmentSettings.siteHost)

$siteAndUriPath = $($environmentSettings.siteHost) + "/" + $($environmentSettings.appName)

Format-TaskNameToHost "Setting IIS Authentication for $($siteAndUriPath)"
Set-IISAuthentication windowsAuthentication false $($siteAndUriPath)
Set-IISAuthentication anonymousAuthentication true $($siteAndUriPath)

Write-Host -Fore Green "Successfully deployed Web Application"
