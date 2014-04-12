<#
    .DESCRIPTION
        Transform the App.config and Web.config files using the supplied tranform.

    .EXAMPLE
        Todo

    .PARAMETER xml
        Todo

    .PARAMETER xdt
        Todo

    .SYNOPSIS
        Todo

    .NOTES
        Nothing yet...
#>

function Invoke-ConfigTransform
{
    param(
        [parameter(Mandatory=$true,position=0)] [string] $transformDll,
        [parameter(Mandatory=$true,position=0)] [string] $src,
        [parameter(Mandatory=$true,position=1)] [string] $xdt
    )

    $scriptPath = Get-ModuleDirectory

    Add-Type -Path $transformDll

    try 
    {
        $doc = New-Object Microsoft.Web.XmlTransform.XmlTransformableDocument
        $doc.PreserveWhiteSpace = $true
        $doc.Load($src)

        $trn = New-Object Microsoft.Web.XmlTransform.XmlTransformation($xdt)

        if ($trn.Apply($doc))
        {
            $doc.Save($src)
            Write-Output "Output file: $doc"
        }
        else
        {
            throw "Transformation terminated with status False"
        }
    }
    catch
    {
        Write-Output $Error[0].Exception
    }
}

function Get-ModuleDirectory {
    return Split-Path $script:MyInvocation.MyCommand.Path
}

Export-ModuleMember Invoke-ConfigTransform