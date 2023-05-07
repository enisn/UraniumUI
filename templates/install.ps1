dotnet pack -c Release -o .\nupkg -p:packageVersion=0.0.1

dotnet new uninstall UraniumUI.Templates
dotnet new install .\nupkg\UraniumUI.Templates.0.0.1.nupkg