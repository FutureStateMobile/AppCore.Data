<#
    .DESCRIPTION
        Will parse an XML config file and replace the values at a xpath expression with the value passed in.

    .EXAMPLE
        Update-ConfigValues "C:\temp\somefile.config" "//SomeNode/AnotherNode" "Some New Value"

    .PARAMETER configFile
        A path to a file that is XML based

    .PARAMETER xpath
        Any valid XPath exression, wether result in 1 or many matches, wether a Element or and Attribute.

    .PARAMETER value
        Any valid XML value that you wish to set.

    .SYNOPSIS
        Updates a XML file with the value specified at the XPath expression specified..

    .NOTES
        Nothing yet...
#>
function Update-ConfigValues
{
    param( 
        [parameter(Mandatory=$true,position=0)] [string] $configFile,
        [parameter(Mandatory=$true,position=1)] [string] $xpath,
        [parameter(Mandatory=$true,position=2)] [string] $value,
        [parameter(Mandatory=$false,position=3)] [string] $attributeName
    )

    $ErrorActionPreference = "Stop"

    $doc = New-Object System.Xml.XmlDocument;
    $doc.Load($configFile)

    $nodes = $doc.SelectNodes("$xpath")

    foreach ($node in $nodes) {
        if ($node -ne $null) {
            if ($attributeName) {
                if ($node.HasAttribute($attributeName)) {
                    $node.SetAttribute($attributeName, $value)
                    Write-Host "Updated '$xpath->$attributeName' to: $value"
                } else {
                    Write-Host "$attributeName wasn't found"
                }
            } else {
                if ($node.NodeType -eq "Element") {
                    $node.InnerXml = $value
                }
                else {
                    $node.Value = $value
                }
                Write-Host "Updated '$xpath' to: $value"
            }
        }
    }

    $doc.Save($configFile)
}

Export-ModuleMember Update-ConfigValues