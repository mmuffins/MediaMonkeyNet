param (
[string]$ProjectPath
)

# used in pre-build command as
#powershell -NoProfile -ExecutionPolicy Bypass -NonInteractive -file $(ProjectDir)PreBuild.ps1 -ProjectPath $(ProjectPath)

#Read csproj (XML)
$xml = [xml](Get-Content $ProjectPath)

#Retieve Version Nodes
$assemblyVersion = $xml.Project.PropertyGroup.AssemblyVersion
$fileVersion = $xml.Project.PropertyGroup.FileVersion

#Split the Version Numbers
$avMajor, $avMinor, $avBuild  = $assemblyVersion.Split(".")
$fvMajor, $fvMinor, $fvBuild = $fileVersion.Split(".")

#Increment Revision
$avBuild = [Convert]::ToInt32($avBuild,10)+1
$fvBuild = [Convert]::ToInt32($fvBuild,10)+1

#Put new version back into csproj (XML)
$xml.Project.PropertyGroup.AssemblyVersion = "$avMajor.$avMinor.$avBuild"
$xml.Project.PropertyGroup.FileVersion = "$fvMajor.$fvMinor.$fvBuild"

#Save csproj (XML)
$xml.Save($ProjectPath)