$resourceName = "rg-wenco-13"
$redisCacheName = "wenco-redis-db-13"
$locationName = "East US"

$AppServicePlan="DemoWebApps-13"
$webAppName = "wenco-webapp-13"

$cosmosAccountName = "wenco-cosmo-testdb-13"
$cosmosDatabaseName = "wenco-cosmoDB"
$cosmosContainerName = "wenco-cosmoDB-container"
$cosmosPartionKey = "Site"
    
$sqlServerName = "wenco-server-13"
$sqlUserName = "wencosqlserver"
$sqlPassword = "Wenco#12345"
$sqlDatabaseName = "wencodb"
$sqlServerFirewallRule = "wencoFirewallRule"

$sourceRootPath="F:\Wenco\Git\POCS\GeoFancing"
$powerShellFolder = "$sourceRootPath\Powershell"

$sqlqueryFilePath = "$powerShellFolder\SqlQuery.sql"

$publishFolder ="$sourceRootPath\Hosts\WebAppHost\bin\Release\netcoreapp2.2\publish"



$appSettingFilePath = "$publishFolder\appsettings.json"
$webAppSource = "$publishFolder\*"
$webAppDestination = "$publishFolder\runtimes.zip"

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
& "$powerShellFolder\RedisCache.ps1" -resourceName $resourceName -redisCacheName $redisCacheName -locationName $locationName

Write-Host "Build CosmoDb Server"
& "$powerShellFolder\CosmoDB.ps1" -resourceName $resourceName -locationName $locationName -accountName $cosmosAccountName -databaseName $cosmosDatabaseName -containerName $cosmosContainerName -partitionkey $cosmosPartionKey

Write-Host "Build SQL Azure Database"
& "$powerShellFolder\SQLAzure.ps1" -resourceName $resourceName -locationName $locationName -sqlServerName $sqlServerName -sqlUserName $sqlUserName -sqlPassword $sqlPassword -sqlDatabaseName $sqlDatabaseName -sqlServerFirewallRule $sqlServerFirewallRule -queryFilePath $sqlqueryFilePath

Write-Host "Update App.Settings.Json File"
& "$powerShellFolder\UpdateAppSettingsJson.ps1" -resourceName $resourceName -locationName $locationName -filePath $appSettingFilePath -redisCacheName $redisCacheName -cosmoAccountName $cosmosAccountName -cosmoDatabaseName $cosmosDatabaseName -cosmoContainerName $cosmosContainerName -sqlServerName $sqlServerName -sqlDatabaseName $sqlDatabaseName -sqlUserName $sqlUserName -sqlPassword $sqlPassword

Write-Host "Building Web App"
& "$powerShellFolder\WebApp.ps1" -resourceName $resourceName -locationName $locationName -AppServicePlan $AppServicePlan -webAppName $webAppName -source $webAppSource -destination $webAppDestination

#open web app
start "https://$webAppName.azurewebsites.net/all"

#Ask to delete the complete the resoucre group
$confirmToDelete = Read-Host -Prompt "Please confirm (Y/N) to delete all the resources" 
if($confirmToDelete.Equals("Y") -or $confirmToDelete -eq "y")
{
    Get-AzResourceGroup -Name $resourceName | Remove-AzResourceGroup -Verbose -Force
}
    