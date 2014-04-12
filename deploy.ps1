<#
	.SYNOPSIS
		Deploys the most resent build of AudioBook.

	.Description
		This script looks for a AudioBook.nupkg file within the build-artifacts folder, if found it unpacks it and runs the deploy.ps1 file to deploy to your local IIS server.
		
		It also handles adding a hosts entry to point 'local.fsmobile.ca' to your local host.

	.EXAMPLE
		.\deploy
		You can just call the deploy script and this will excute with all the defaults.

	.EXAMPLE
		.\deploy -Environment development
		This will call the deploy script using the development servers settings.

	.PARAMETER Version
		This specifies the version of the package you are deploying, only specifiy this paramter if you have a specific versioned package you will to deploy.

	.PARAMETER Environment
		Used to define what environment settings file you wish to you for deployment, defaults to "local".

	.LINK
		http://learn.iis.net/page.aspx/150/understanding-sites-applications-and-virtual-directories-on-iis/
#>
param(
    [Parameter(Position=1)] [string] $Version = "0.1.1.1",
    [Parameter(Position=0)] [string] $Environment = "local"
)

$ErrorActionPreference = "Stop"

$currentDir = Split-Path $script:MyInvocation.MyCommand.Path

Import-Module "$currentDir\build\modules\Test-RunAsAdmin"
Test-RunAsAdmin

Import-Module $currentDir\build\modules\Get-EnvironmentSettings
$environmentSettings = Get-EnvironmentSettings $Environment "//environmentSettings"

Import-Module "$currentDir\build\modules\Add-LoopbackFix"
Add-LoopbackFix $($environmentSettings.siteHost)

Import-Module "$currentDir\build\modules\Add-HostsFileEntry"
Add-HostsFileEntry "127.0.0.1" $($environmentSettings.siteHost)

$BuildArtifactsPath = (Join-Path $currentDir "build-artifacts")
$NameAndVersion = "FutureState.BreathingRoom.Server." + $Version
$PackagePath = (Join-Path $BuildArtifactsPath $NameAndVersion)
$NupkgPath = $PackagePath + ".nupkg"

if (test-path $NupkgPath)
{
	Write-Host $PackagePath
	Write-Host $NupkgPath
	
	# Create Folder for the application
	if(!(Test-Path $PackagePath)) {
		md $PackagePath | Out-Null
	}
	else {
		Remove-Item "$PackagePath\*" -recurse -Force
	}
	
	Write-Host "Unpacking NuGet file..."
	Import-Module "$currentDir\build\modules\Expand-NugetPackage"
	Expand-NugetPackage $NupkgPath $PackagePath

	Write-Host "Deploying Mobi.FutureState.BreathingRoom.Server..."
	& "$PackagePath\Deploy.ps1" $Version $Environment

	Write-Host ""
	Write-Host "Attempting to Restart IIS"
	IISRESET
}
else
{
	Write-Host "Could not find $NupkgPath" -ForegroundColor Red
    Write-Host " -- Please run .\Build.ps1 before trying to deploy." -ForegroundColor Red
}