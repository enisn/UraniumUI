param (
    [string]$version = "1.0.0",
    [switch]$push = $false,
    [string]$apikey = "",
    [string]$source = "nuget.org"
 )

Invoke-Expression "dotnet clean"

if (!$skipbuild) {
    Invoke-Expression "dotnet pack -c Release -o . -p:packageVersion=$version"
}
if ($push) {
    Invoke-Expression "dotnet nuget push '*.$version.nupkg' --api-key $apikey --skip-duplicate --source $source"
}

Set-Location ../ # back to root