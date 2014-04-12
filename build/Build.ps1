properties {
    $environment =  if ("$env".length -gt 0) { "$env" } else { 'local' }
    $framework = '4.0'
    $rootDir = (Resolve-Path $pwd\..\)
    $solutionFile = (Resolve-Path $rootDir\*.sln)
    $testAssembly = "FutureState.AppCore.Tests"
    $appServerAssembly = "FutureState.BreathingRoom.Server"
    $androidAssembly = "FutureState.BreathingRoom.Droid.Ui"
    $androidApkName = "mobi.futurestate.breathingroom"
    $appTestsProjectFile = (Resolve-Path $rootDir\$testAssembly\*.csproj)
    $appServerProjectFile = (Resolve-Path $rootDir\$appServerAssembly\*.csproj)
    $androidProjectFile = (Resolve-Path $rootDir\$androidAssembly\*.csproj)
    $testProjectDir = (Resolve-Path $rootDir\$testAssembly)
    $buildDir = (Resolve-Path $rootDir\build)
    $buildArtifactsDir = (Join-Path $rootDir build-artifacts)
    $buildOutputDir = (Join-Path $buildArtifactsDir output)
    $buildPublishDir = (Join-Path $buildArtifactsDir publish)
    $nuspecFile = (Resolve-Path $buildDir\FutureState.BreathingRoom.Server.nuspec)
    $packagesDir = (Resolve-Path $rootDir\packages)
    $version = if ("$version".length -gt 0) { "$version" } else { '1.0.0' }
    $buildNumber = if ("$buildNumber".length -gt 0) { "$buildNumber" } else { '1' }
    $informationalVersion = if ("$informationalVersion".length -gt 0) { "$informationalVersion" } else { 'Developer Build' }
    $nUnitVersion = "2.6.3"
    $nuGetVersion = "2.7.3"
}

task default -Depends Clean, Compile, UnitTest, IntegrationTest, FunctionalTest, Package, BuildAndroidApk, PackageForAndroid, PostPackageCleanup
#task default -Depends Clean, Compile, UnitTest, IntegrationTest, FunctionalTest, Package, PostPackageCleanup

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

task SetAndroidVersion -Description "Set the versionCode and versionName in the AndroidManifest file" {
    Import-Module "$buildDir\modules\Update-AndroidManifest"
    Update-AndroidManifest $version $buildNumber
}

task MakeBuildDir -Description "Creates the build artifacts directory" {
    if (-not (Test-Path $buildArtifactsDir)) {
        New-Item $buildArtifactsDir -ItemType Container
    }
}

task Compile -Depends Clean, MakeBuildDir, SetVersion -Description "Compiles the application" {
    # There is a bug in msbuild that doesn't allow us to specify the buildnumber or assembly version at the command CommandLine
    # Such as: "/p:ApplicationRevision=$buildNumber" "/p:ApplicationVersion=$version"

    Exec { msbuild $appTestsProjectFile "/p:Configuration=Release" "/p:OutDir=$buildOutputDir\" } "Build Failed - Compilation"
}

task CopyAppServer -Depends SetVersion {
    Exec { msbuild $appServerProjectFile "/t:Rebuild" "/t:ResolveReferences" "/t:_CopyWebApplication" "/p:Configuration=Release" "/p:WebProjectOutputDir=$buildPublishDir\_PublishedWebsites\$appServerAssembly\" "/p:OutDir=$buildOutputDir\" } "Build Failed - Copy Server"
}

task BuildAndroidApk -Depends Clean, MakeBuildDir, SetAndroidVersion -Description "Builds and packages the Android application" {
    Exec { msbuild $androidProjectFile /p:Configuration=Release /p:OutDir=$buildOutputDir\ /t:PackageForAndroid } "Build Failed - BuildAndroidApk"
}

task UnitTest -Depends Compile -Description "Runs only Unit Tests" {
    $targetDll = Resolve-Path $buildOutputDir\$testAssembly.dll
    $xmlFile = "$buildArtifactsDir\UnitTest-Results.xml"
  
    Exec { & $packagesDir\NUnit.Runners.$nUnitVersion\tools\nunit-console.exe /run:$testAssembly.Unit "$targetDll" /xml="$xmlFile" } "Unit Tests Failed"
}

task IntegrationTest -Depends Compile -Description "Runs only Integration tests" {
    $targetDll = Resolve-Path $buildOutputDir\$testAssembly.dll
    $xmlFile = "$buildArtifactsDir\IntegrationTest-Results.xml"
  
    Import-Module "$buildDir\modules\Get-EnvironmentSettings"
    $databaseSettings = Get-EnvironmentSettings $environment "//serverDatabase"

    $testAssemblyConfigXPath = "/configuration/connectionStrings/add"

    Import-Module "$buildDir\modules\Update-XmlConfigFile.psm1"
    Update-ConfigValues "$($buildOutputDir)\$testAssembly.dll.config" $testAssemblyConfigXPath $($databaseSettings.connectionString) "connectionString"
  
    Exec { & $packagesDir\NUnit.Runners.$nUnitVersion\tools\nunit-console.exe /run:$testAssembly.Integration "$targetDll" /xml="$xmlFile" } "Integration Tests Failed"
}

task FunctionalTest -Depends Compile -Description "Runs only Functional tests" {
    $targetDll = Resolve-Path $buildOutputDir\$testAssembly.dll
    $xmlFile = "$buildArtifactsDir\FunctionalTest-Results.xml"
  
    Import-Module "$buildDir\modules\Get-EnvironmentSettings"
    $databaseSettings = Get-EnvironmentSettings $environment "//serverDatabase"

    $testAssemblyConfigXPath = "/configuration/connectionStrings/add"

    Import-Module "$buildDir\modules\Update-XmlConfigFile.psm1"
    Update-ConfigValues "$($buildOutputDir)\$testAssembly.dll.config" $testAssemblyConfigXPath $($databaseSettings.connectionString) "connectionString"
  
    Exec { & $packagesDir\NUnit.Runners.$nUnitVersion\tools\nunit-console.exe /run:$testAssembly.Functional "$targetDll" /xml="$xmlFile" } "Functional Tests Failed"
}

task SetPackageVersion -Description "Task which sets the proper version information for the Nuget package" {
    Import-Module "$buildDir\modules\Update-XmlConfigFile.psm1"
    Update-ConfigValues $nuspecFile "//*[local-name() = 'version']" $version
    Update-ConfigValues $nuspecFile "//*[local-name() = 'summary']" "FutureState.AudioBook $($informationalVersion)"
}

task Package -Depends CopyAppServer, SetPackageVersion -Description "Task which bundles the build artifacts into a NuGet package" {
    Exec { & $packagesDir\NuGet.CommandLine.$nuGetVersion\tools\nuget.exe pack $nuspecFile -Version "$($version).$($buildNumber)" -OutputDirectory $buildArtifactsDir -NoPackageAnalysis }
}

task PackageForAndroid -Description "Get the Android apk ready for deployment" {
    # At this point there is only the unsigned APK - sign it.
    # The script will pause here as jarsigner prompts for the password.
    # It is possible to provide they keystore password for jarsigner.exe by adding an extra command line parameter -storepass, for example
    #    -storepass <MY_SECRET_PASSWORD>
    # If this script is to be checked in to source code control then it is not recommended to include the password as part of this script.
    if (! (Test-Path env:JAVA_HOME)) {
        Write-Host "Please install the Java JDK and set your JAVA_HOME environment variable."
        exit 1        
    }

    $javaHome = (Get-Item env:JAVA_HOME).Value
    Exec { & "$javaHome\bin\jarsigner.exe" -verbose -sigalg SHA1withRSA -digestalg SHA1 -storepass Y3110Wd0g -keystore $buildDir/certs/cosmicgame.keystore -signedjar $buildOutputDir/$androidApkName-signed.apk $buildOutputDir/$androidApkName.apk cosmicgame } "Signing the Android apk Failed"

    # Now zipalign it.  The -v parameter tells zipalign to verify the APK afterwards.
    Exec { & "$buildDir\modules\zipalign.exe" -f -v 4 $buildOutputDir/$androidApkName-signed.apk $buildArtifactsDir/$androidApkName.$version.$buildNumber.apk } "Zipaligning the Android apk Failed"
    #Copy-Item $buildOutputDir/$androidApkName-signed.apk $buildArtifactsDir/$androidApkName.$version.$buildNumber.apk
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