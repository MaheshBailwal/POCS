$resourceName = "rg-wenco-performance-test-monitoring"
$redisCacheName = "wenco-redis-db12"
$locationName = "East US"

$AppServicePlan="DemoWebApps12"
$webAppName = "wenco-webapp12"

$cosmosAccountName = "wenco-cosmo-testdb12"
$cosmosDatabaseName = "wenco-cosmoDB"
$cosmosContainerName = "wenco-cosmoDB-container"
$cosmosPartionKey = "Site"
    
$sqlServerName = "wenco-server12"
$sqlUserName = "wencosqlserver"
$sqlPassword = "Wenco#12345"
$sqlDatabaseName = "wencodb"
$sqlServerFirewallRule = "wencoFirewallRule"

$sourceRootPath="F:\Wenco\Git\POCS\GeoFancing"

$sqlqueryFilePath = "$PSScriptRoot\SqlQuery.sql"

$publishFolder ="$sourceRootPath\Hosts\WebAppHost\bin\Debug\netcoreapp2.2\publish"

$appSettingFilePath = "$publishFolder\appsettings.json"
$webAppSource = "$publishFolder\*"
$webAppDestination = "$sourceRootPath\Hosts\WebAppHost\bin\Debug\runtimes.zip"

<#-----------------------
https://docs.microsoft.com/en-us/powershell/azure/install-az-ps?view=azps-3.2.0
To run Azure PowerShell in PowerShell 5.1 on Windows:

Update to Windows PowerShell 5.1 if needed. If you're on Windows 10, you already have PowerShell 5.1 installed.
Install .NET Framework 4.7.2 or later.

-----------------------#>


# Check the NuGetPackage Provide module and installed
Install-PackageProvider -Name NuGet -Force

# Check the Az Module and installed
$azModule = Get-InstalledModule -Name Az -ErrorAction SilentlyContinue
if(-not $azModule)
{
    Install-Module -Name Az -RequiredVersion 3.0 -Force -AllowClobber
}

# Check the current azure session
$azContext = Get-AzContext
#echo $content
if (-not $azContext) 
{
	Login-AzAccount
}

Write-Host "Checking Resource Group existence"
$resourceGroup = Get-AzResourceGroup -Name $resourceName -ErrorAction SilentlyContinue
if([string]::IsNullOrEmpty($resourceGroup.ResourceGroupName))
{
    Write-Host "Creating Resource Group."
    New-AzResourceGroup -Name $resourceName -Location $locationName
}

Write-Host "Build Redis Cache"
& "$PSScriptRoot\RedisCache.ps1" -resourceName $resourceName -redisCacheName $redisCacheName -locationName $locationName

Write-Host "Build CosmoDb Server"
& "$PSScriptRoot\CosmoDB.ps1" -resourceName $resourceName -locationName $locationName -accountName $cosmosAccountName -databaseName $cosmosDatabaseName -containerName $cosmosContainerName -partitionkey $cosmosPartionKey

Write-Host "Build SQL Azure Database"
& "$PSScriptRoot\SQLAzure.ps1" -resourceName $resourceName -locationName $locationName -sqlServerName $sqlServerName -sqlUserName $sqlUserName -sqlPassword $sqlPassword -sqlDatabaseName $sqlDatabaseName -sqlServerFirewallRule $sqlServerFirewallRule -queryFilePath $sqlqueryFilePath

Write-Host "Build Blob Storage"
& "$PSScriptRoot\BlobStorage.ps1" -resourceName $resourceName -locationName $locationName -blobStorageName $blobStorageName

Write-Host "Update App.Settings.Json File"
& "$PSScriptRoot\UpdateAppSettingsJson.ps1" -resourceName $resourceName -locationName $locationName -filePath $appSettingFilePath -redisCacheName $redisCacheName -cosmoAccountName $cosmosAccountName -cosmoDatabaseName $cosmosDatabaseName -cosmoContainerName $cosmosContainerName -sqlServerName $sqlServerName -sqlDatabaseName $sqlDatabaseName -sqlUserName $sqlUserName -sqlPassword $sqlPassword

Write-Host "Building Web App"
& "$PSScriptRoot\WebApp.ps1" -resourceName $resourceName -locationName $locationName -AppServicePlan $AppServicePlan -webAppName $webAppName -source $webAppSource -destination $webAppDestination

#open web app
start "https://$webAppName.azurewebsites.net/RunTest"

#Ask to delete the complete the resoucre group
$confirmToDelete = Read-Host -Prompt "Please confirm (Y/N) to delete all the resources" 
if($confirmToDelete.Equals("Y") -or $confirmToDelete -eq "y")
{
    Get-AzResourceGroup -Name $resourceName | Remove-AzResourceGroup -Verbose -Force
}
    