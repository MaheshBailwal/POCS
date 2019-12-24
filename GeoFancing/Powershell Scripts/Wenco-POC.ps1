$resourceName = "rg-wenco"
$redisCacheName = "wenco-redis-db"
$locationName = "East US"

$AppServicePlan="DemoWebApps"
$webAppName = "wenco-webapp"

$cosmosAccountName = "wenco-cosmo-testdb"
$cosmosDatabaseName = "wenco-cosmoDB"
$cosmosContainerName = "wenco-cosmoDB-container"
$cosmosPartionKey = "Site"

$sqlServerName = "wenco-server"
$sqlUserName = "wencosqlserver"
$sqlPassword = "Wenco#12345"
$sqlDatabaseName = "wencodb"
$sqlServerFirewallRule = "wencoFirewallRule"
$sqlqueryFilePath = "F:\AzureTemplates\SqlQuery.sql"

$appSettingFilePath = "F:\AzureTemplates\WebApp\PublishProject\appsettings.json"
$webAppSource = "F:\AzureTemplates\WebApp\PublishProject\*"
$webAppDestination = "F:\AzureTemplates\WebApp\PublishProject\runtimes.zip"

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
& "F:\AzureTemplates\RedisCache.ps1" -resourceName $resourceName -redisCacheName $redisCacheName -locationName $locationName

Write-Host "Build CosmoDb Server"
& "F:\AzureTemplates\CosmoDB.ps1" -resourceName $resourceName -locationName $locationName -accountName $cosmosAccountName -databaseName $cosmosDatabaseName -containerName $cosmosContainerName -partitionkey $cosmosPartionKey

Write-Host "Build SQL Azure Database"
& "F:\AzureTemplates\SQLAzure.ps1" -resourceName $resourceName -locationName $locationName -sqlServerName $sqlServerName -sqlUserName $sqlUserName -sqlPassword $sqlPassword -sqlDatabaseName $sqlDatabaseName -sqlServerFirewallRule $sqlServerFirewallRule -queryFilePath $sqlqueryFilePath

Write-Host "Update App.Settings.Json File"
& "F:\AzureTemplates\UpdateAppSettingsJson.ps1" -resourceName $resourceName -locationName $locationName -filePath $appSettingFilePath -redisCacheName $redisCacheName -cosmoAccountName $cosmosAccountName -cosmoDatabaseName $cosmosDatabaseName -cosmoContainerName $cosmosContainerName -sqlServerName $sqlServerName -sqlDatabaseName $sqlDatabaseName -sqlUserName $sqlUserName -sqlPassword $sqlPassword

Write-Host "Building Web App"
& "F:\AzureTemplates\WebApp.ps1" -resourceName $resourceName -locationName $locationName -AppServicePlan $AppServicePlan -webAppName $webAppName -source $webAppSource -destination $webAppDestination

#open web app
start ‘https://wenco-webapp.azurewebsites.net/cosmo’

#Ask to delete the complete the resoucre group
$confirmToDelete = Read-Host -Prompt "Please confirm (Y/N) to delete all the resources" 
if($confirmToDelete.Equals("Y") -or $confirmToDelete -eq "y")
{
    Get-AzResourceGroup -Name $resourceName | Remove-AzResourceGroup -Verbose -Force
}
    