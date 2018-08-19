$apiKey = Read-Host "Please enter your nuget api key"

if([string]::IsNullOrEmpty($apiKey)){
    return
}

$package = get-childitem -path .\bin\release -file -filter *.nupkg | sort-object creationtimeutc -descending | select-object -first 1
nuget push "$($package.fullname)" -apikey $apikey -source nuget.org

Read-Host
