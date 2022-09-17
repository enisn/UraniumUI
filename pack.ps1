param (
    [string]$version = "1.0.0",
    [switch]$push = $false,
    [Switch]$skipbuild = $false,
    [string]$apikey = "",
    [string]$source = "nuget.org"
 )

 if (!$skipbuild) {
      Invoke-Expression "dotnet pack -c Release -o . -p:packageVersion=$version --include-symbols"
}

if ($push) {
    Invoke-Expression "dotnet nuget push '*.$version.symbols.nupkg' --api-key $apikey --skip-duplicate --source $source"
}

Set-Location ./templates

if (!$skipbuild) {
    Invoke-Expression "dotnet pack -c Release -o . -p:packageVersion=$version"
}
if ($push) {
    Invoke-Expression "dotnet nuget push '*.$version.nupkg' --api-key $apikey --skip-duplicate --source $source"
}

Set-Location ../ # back to root