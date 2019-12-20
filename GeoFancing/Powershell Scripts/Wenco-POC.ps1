$resourceName = "rg-wenco-poc"
$redisCacheName = "wenco-redis-db"
$webAppName = "wenco-webapp"
$locationName = "East US"
$AppServicePlan="DemoWebApps"

$redisTemplateLocation = "F:\AzureTemplates\RedisScriptAndJson\redistemplate.json"
$publishPackage = "F:\AzureTemplates\WebApp\PublishProject\runtimes.zip"
$filePath = "F:\AzureTemplates\WebApp\PublishProject\appsettings.json"
$source = "F:\AzureTemplates\WebApp\PublishProject\*"


Write-Host "Checking Resource Group existence"
$resourceGroup = Get-AzResourceGroup -Name $resourceName -ErrorAction SilentlyContinue
if([string]::IsNullOrEmpty($resourceGroup.ResourceGroupName))
{
    Write-Host "Creating Resource Group."
    New-AzResourceGroup -Name $resourceName -Location $locationName
}

Write-Host "Checking Redis cache existence"
$redisCache = Get-AzRedisCache -ResourceGroupName $resourceName -Name $redisCacheName -ErrorAction SilentlyContinue
if([string]::IsNullOrEmpty($redisCache.Name))
{
    Write-Host "Creating Redis cache."
    New-AzResourceGroupDeployment -ResourceGroupName $resourceName -TemplateFile $redisTemplateLocation
    $redisCache = Get-AzRedisCache -ResourceGroupName $resourceName -Name $redisCacheName -ErrorAction SilentlyContinue

    Write-Host "Update Redis Connection string into appSettings.json file"
    $redisCacheKey = Get-AzRedisCacheKey -Name $redisCacheName -ResourceGroupName $resourceName
    $redisConnectionstring = "wenco-redis-db.redis.cache.windows.net:6380,password=" + $redisCacheKey.PrimaryKey +",ssl=True,abortConnect=False"
    $file = ([System.IO.File]::ReadAllText($filePath)  | ConvertFrom-Json)
    $file.AppSettings.RedisCacheConfig = $redisConnectionstring
    $file | ConvertTo-Json | Out-File -FilePath $filePath -Encoding utf8 -Force    
}

Write-Host "Comressing file into zip file"
Compress-Archive -Path $source -Update -DestinationPath $destination
    
Write-Host "Checking App service plan existence"
$appServicePlan = Get-AzAppServicePlan -Name $AppServicePlan -ResourceGroupName $resourceName -ErrorAction SilentlyContinue
if([string]::IsNullOrEmpty($AppServicePlan.Length))
{
    Write-Host "Creating App Service plan."
    New-AzAppServicePlan -Name $AppServicePlan -Location $location -ResourceGroupName $resourceName -Tier Free
    $appServicePlan = Get-AzAppServicePlan -Name $AppServicePlan -ResourceGroupName $resourceName -ErrorAction SilentlyContinue
}

Write-Host "Checking web app existence"
$webApp = Get-AzWebApp -ResourceGroupName $resourceName -Name $webAppName
if([string]::IsNullOrEmpty($webApp))
{
    Write-Host "Creating web app."
    New-AzWebApp -Name $webAppName -Location $location -AppServicePlan $AppServicePlan -ResourceGroupName $resourceName 
    $webApp = Get-AzWebApp -ResourceGroupName $resourceName -Name $webAppName
}

#Publish web app
Write-Host "publishing package on the web app"
Publish-AzWebapp -ResourceGroupName $resourceName -Name $webAppName -ArchivePath $destination

#open web app
start ‘https://wenco-webapp.azurewebsites.net/redis’

#Ask to delete the complete the resoucre group
$confirmToDelete = Read-Host -Prompt "Please confirm (Y/N) to delete all the resources" 
if($confirmToDelete.Equals("Y") -or $confirmToDelete -eq "y")
{
    Get-AzResourceGroup -Name $resourceName | Remove-AzResourceGroup -Verbose -Force
}
    