<#
    .DESCRIPTION
        Scopes the use of a disposable with a script block

    .EXAMPLE
        Using ($user = $sr.GetDirectoryEntry()) { 
            $user.displayName = $displayName 
            $user.SetInfo() 
        } 

    .PARAMETER inputObject
        The object to be disposed of

    .SYNOPSIS
        Will implement a statement similar to the Using statement in C#

    .LINK
        http://weblogs.asp.net/adweigert/archive/2008/08/27/powershell-adding-the-using-statement.aspx
#>

function Invoke-Using
{
    param (
        [parameter(Mandatory=$true,position=0)] [System.IDisposable] $inputObject,
        [parameter(Mandatory=$true,position=1)] [ScriptBlock] $scriptBlock
    )
    
    Try
    {
        &$scriptBlock
    }
    Finally
    {
        if ($inputObject -ne $null)
        {
            if ($inputObject.psbase -eq $null) 
            {
                $inputObject.Dispose()
            }
            else 
            {
                $inputObject.psbase.Dispose()
            }
        }
    }
}

Export-ModuleMember Invoke-Using