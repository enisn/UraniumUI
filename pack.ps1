param (
    [string]$version = "1.0.0",
    [switch]$push = $false,
    [Switch]$skipbuild = $false,
    [string]$apikey = "",
    [string]$source = "nuget.org"
 )

 if (!$skipbuild) {
    Write-Host "UraniumUI packages packing started."

    Invoke-Expression "dotnet pack -c Release -o . -p:packageVersion=$version --include-symbols"
    
    Write-Host "UraniumUI packages packing completed."
}

if ($push) {
    Write-Host "UraniumUI packages pushing started."

    Invoke-Expression "dotnet nuget push '*.$version.symbols.nupkg' --api-key $apikey --skip-duplicate --source $source"
    Write-Host "UraniumUI packages has been pushed successfully." -ForegroundColor Green
}

Set-Location ./templates

if (!$skipbuild) {
    Write-Host "UraniumUI templates packing started."
    Invoke-Expression "dotnet pack -c Release -o . -p:packageVersion=$version"
    Write-Host "UraniumUI templates packing completed."
}
if ($push) {
    Write-Host "UraniumUI templates pushing started."
    Invoke-Expression "dotnet nuget push '*.$version.nupkg' --api-key $apikey --skip-duplicate --source $source"
    Write-Host "UraniumUI templates has been pushed successfully." -ForegroundColor Green
}

Set-Location ../ # back to root