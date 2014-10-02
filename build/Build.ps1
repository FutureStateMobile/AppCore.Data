properties {
    $environment =  if ("$env".length -gt 0) { "$env" } else { 'local' }
    $framework = '4.0'
    $rootDir = (Resolve-Path $pwd\..\)
    $solutionFile = (Resolve-Path $rootDir\*.sln)
    $testAssembly = "FutureState.AppCore.Data.Tests"
    $appTestsProjectFile = (Resolve-Path $rootDir\$testAssembly\*.csproj)
    $testProjectDir = (Resolve-Path $rootDir\$testAssembly)
    $buildDir = (Resolve-Path $rootDir\build)
    $buildArtifactsDir = (Join-Path $rootDir build-artifacts)
    $buildOutputDir = (Join-Path $buildArtifactsDir output)
    $buildPublishDir = (Join-Path $buildArtifactsDir publish)
    $nuspecFile = (Resolve-Path $buildDir\FutureState.AppCore.Data.nuspec)
    $packagesDir = (Resolve-Path $rootDir\packages)
    $version = if ("$version".length -gt 0) { "$version" } else { '1.0.0' }
    $buildNumber = if ("$buildNumber".length -gt 0) { "$buildNumber" } else { '1' }
    [string[]] $symbolsToCopy = @("FutureState.AppCore.Data.pdb", "FutureState.AppCore.Data.Sqlite.pdb","FutureState.AppCore.Data.Sqlite.Windows.pdb","FutureState.AppCore.Data.SqlServer.pdb")
    $informationalVersion = if ("$informationalVersion".length -gt 0) { "$informationalVersion" } else { 'Developer Build' }
    $nUnitVersion = "2.6.3"
    $nuGetVersion = "2.7.3"
}

task default -Depends Clean, Compile, UnitTest, IntegrationTest, CopySymbols, Package #, PostPackageCleanup

FormatTaskName {
    param($taskName)
    Import-Module "$buildDir\modules\Format-TaskNameToHost"

    Format-TaskNameToHost $taskName
}

task Clean -Description "Deletes the build artifacts directory and runs a 'clean' on the solution" { 
    if (Test-Path $buildArtifactsDir) {
        Remove-Item $buildArtifactsDir -Recurse -Force -ErrorAction SilentlyContinue
    }
    Exec { msbuild $solutionFile /:Configuration=Release /t:clean } "Could not clean the project"
}

task SetVersion -Description "Sets the version number in 'AssemblyInfo.cs'" {
    Import-Module "$buildDir\modules\Update-AssemblyVersions"
    Update-AssemblyVersions $version $buildNumber $informationalVersion
}

task MakeBuildDir -Description "Creates the build artifacts directory" {
    if (-not (Test-Path $buildArtifactsDir)) {
        New-Item $buildArtifactsDir -ItemType Container
    }
}

task Compile -Depends Clean, MakeBuildDir, SetVersion -Description "Compiles the application" {
    # There is a bug in msbuild that doesn't allow us to specify the buildnumber or assembly version at the command CommandLine
    # Such as: "/p:ApplicationRevision=$buildNumber" "/p:ApplicationVersion=$version"

    Exec { msbuild $solutionFile "/p:Configuration=Release" "/p:OutDir=$buildOutputDir\" } "Build Failed - Compilation"
}

task UnitTest -Depends Compile -Description "Runs only Unit Tests" {
    $targetDll = Resolve-Path $buildOutputDir\$testAssembly.dll
    $xmlFile = "$buildArtifactsDir\UnitTest-Results.xml"
  
    Exec { & $packagesDir\NUnit.Runners.$nUnitVersion\tools\nunit-console.exe /fixture:$testAssembly.Unit $targetDll /xml:$xmlFile } "Unit Tests Failed"
}

task IntegrationTest -Depends Compile -Description "Runs only Integration tests" {
    $targetDll = Resolve-Path $buildOutputDir\$testAssembly.dll
    $xmlFile = "$buildArtifactsDir\IntegrationTest-Results.xml"
  
    Import-Module "$buildDir\modules\Get-EnvironmentSettings"
    $databaseSettings = Get-EnvironmentSettings $environment "//serverDatabase"

    $testAssemblyConfigXPath = "/configuration/connectionStrings/add"

    Import-Module "$buildDir\modules\Update-XmlConfigFile.psm1"
    Update-ConfigValues "$($buildOutputDir)\$testAssembly.dll.config" $testAssemblyConfigXPath $($databaseSettings.connectionString) "connectionString"
  
    Exec { & $packagesDir\NUnit.Runners.$nUnitVersion\tools\nunit-console.exe /fixture:$testAssembly.Integration $targetDll /xml:$xmlFile } "Unit Tests Failed"
}

task SetPackageVersion -Description "Task which sets the proper version information for the Nuget package" {
    Import-Module "$buildDir\modules\Update-XmlConfigFile.psm1"
    Update-ConfigValues $nuspecFile "//*[local-name() = 'version']" $version
    Update-ConfigValues $nuspecFile "//*[local-name() = 'summary']" "FutureState.AppCore.Data $($informationalVersion)"
}

task Package -Depends SetPackageVersion -Description "Task which bundles the build artifacts into a NuGet package" {
    Exec { & $packagesDir\NuGet.CommandLine.$nuGetVersion\tools\nuget.exe pack $nuspecFile -Version "$($version).$($buildNumber)" -OutputDirectory $buildArtifactsDir -NoPackageAnalysis -Symbols }
    PostPackageCleanup
}

task CopySymbols -Depends Compile -Description "Copy symbols to the build artifacts directory"{
    foreach($symbol in $symbolsToCopy)
    {
        Write-Host "Copying $symbol to $buildArtifactsDir"
        Copy-Item $buildOutputDir\$symbol $buildArtifactsDir
    }
}

task ? -Description "Helper to display task info.  In addition to passing a task into the build, you can pass parameters in the form of: -parameters @{env='local',version'X.X.X.X'}" {
    Write-Documentation
}

task PostPackageCleanup {
    if (Test-Path $buildOutputDir) {
        Remove-Item $buildOutputDir -Recurse -Force -ErrorAction SilentlyContinue
    }

    if (Test-Path $buildPublishDir) {
        Remove-Item $buildPublishDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}