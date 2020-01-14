#$project = Read-Host 'Enter csproject filename with path:'
#Write-Host $project
#$output = Read-Host 'Enter release output path:'
#Write-Host $output
$project = "F:\WENCO\POC_VS2019\GeoFancing\Hosts\ConsoleHost"
$output = "F:\WENCO\WencoPublish"
$dotnet = "C:\Program Files\dotnet\dotnet.exe"
$runtimes = @( 
 "win-x64" 
)

<#"win10-x64"
 "win10-x86"
 "win-x64"
 "win-x86"
 "osx-x64"
 "linux-x64"#>
    
# Clear previous releases
 Remove-Item "$output\*" -Recurse -Confirm:$false 

# Publish build
$runtimes | %{
    & $dotnet publish $project -c release -r $_  -o ("{0}\{1}" -f $output,$_)
}