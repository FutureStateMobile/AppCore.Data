<#
    .DESCRIPTION
        Properly returns all the environmental settings for the particualr context in which this build is being run.

    .EXAMPLE
        $dbSettings = Get-EnvironmentSettings "ci" "//database"
        $value = $dbSettings.connectionString

    .PARAMETER environment
        The environment which this build is being run, these environments will match the names of the environments xml config files.  
        If a config file is found that matches the username of which this context is executing, it will use that instead.

    .PARAMETER nodeXPath
        A valid XPath expression that matches the set of values you are after.

    .PARAMETER culture
        If provided will look up settings for an environment based on culture information provided.

    .SYNOPSIS
        Will grab a set of values from the proper environment file and returns them as an object which you can reffer to like any other object.

    .NOTES
        Nothing yet...
#>
function Get-EnvironmentSettings
{
    param(
        [parameter(Mandatory=$true,position=0)] [string] $environment,
        [parameter(Mandatory=$true,position=1)] [string] $nodeXPath = "/",
        [parameter(Mandatory=$false,position=2)] [string] $culture
    )

    $ErrorActionPreference = "Stop"

  	$userName = [Environment]::UserName
    $doc = New-Object System.Xml.XmlDocument
    $currentDir = Get-ModuleDirectory

    if ( $culture ){
        $environmentsDir = (Resolve-Path $currentDir\..\environments\$culture)
    } else {
        $environmentsDir = (Resolve-Path $currentDir\..\environments)
    }

	if (Test-Path "$environmentsDir\$($userName).xml") {
        $doc.Load("$environmentsDir\$($userName).xml")
 	} else {
        $doc.Load("$environmentsDir\$($environment).xml")
	}
    return $doc.SelectSingleNode($nodeXPath);
}

function Get-ModuleDirectory {
    return Split-Path $script:MyInvocation.MyCommand.Path
}

Export-ModuleMember Get-EnvironmentSettings