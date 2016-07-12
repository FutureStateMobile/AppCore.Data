<#
    .SYNOPSIS
        Starts the build process for AppCore.Data.

    .Description
        This script invokes PSake passing it the build definition file located in the 'build' directory.
        
        It also handles constructing the parameters in a much nicer way than PSake takes them by default.

    .EXAMPLE
        .\build
        You can just call the build script and this will excute with all the defaults.

    .EXAMPLE
        .\build UnitTest ci
        In this example you could execute the unit tests as the CI server would.

    .EXAMPLE
        .\build -buildEnvironment "local" -buildNumber "1.2.3.43433"
        If you don't want to specify the parameters in order you reference them explicitly like this.

    .EXAMPLE
        .\build ?
        This will print out a list of all the available tasks that you can execute

    .PARAMETER task
        Used to tell PSake which build task it should execute, this parameter is defaulted to the default task.

    .PARAMETER buildEnvironment
        Used to define which build environment you want to run under. These options are defined in the .\build\environments folder, currently it is limited to [local, ci, dev] with the default being local.

    .PARAMETER buildNumber
        Used to version the package and assemblies.

    .PARAMETER informationalVersion
        Used to give a version string to the package and assemblies.

    .LINK
        https://github.com/psake/psake

    .LINK
        https://github.com/psake/psake/wiki

    .LINK
        http://codebetter.com/jameskovacs/2010/04/12/psake-v4-00/
#>
param(
    [Parameter(Position = 0, Mandatory = 0)][string] $task = "Default",
	[Parameter(Position = 1, Mandatory = 0)][string] $buildEnvironment = "local",
	[Parameter(Position = 2, Mandatory = 0)][string] $buildNumber = "0.1.1.1",
    [Parameter(Position = 3, Mandatory = 0)][string] $informationalVersion = "Developer Build"
)

$buildNumberParts = $buildNumber.split('.')

if ($buildNumberParts.length -ne 4) {
    Write-Host "Incorrectly formated Build Number, it must be formated {X.X.X.X}"
    exit 1
}

$versionNumber = $buildNumberParts[0] + '.' + $buildNumberParts[1] + '.' + $buildNumberParts[2]
$buildNumber = $buildNumberParts[3]
$informationalText = $versionNumber + ' ' + $informationalVersion

& ".\packages\psake.4.6.0\tools\psake.ps1" .\build\Build.ps1 $task -parameters @{
    env=$buildEnvironment;
    version=$versionNumber;
    buildNumber=$buildNumber;
    informationalVersion=$informationalText;
}

if ($psake.build_success -eq $false) { 
    exit 1
}
