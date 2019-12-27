param(
	[Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$resourceName,   
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$locationName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$filePath,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$redisCacheName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$cosmoAccountName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$cosmoDatabaseName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$cosmoContainerName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$sqlServerName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$sqlDatabaseName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$sqlUserName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$sqlPassword
)

$file = ([System.IO.File]::ReadAllText($filePath)  | ConvertFrom-Json)

Write-Host "Update Redis Connection string into appSettings.json file"
$redisCacheKey = Get-AzRedisCacheKey -Name $redisCacheName -ResourceGroupName $resourceName
$redisConnectionstring = $redisCacheName + ".redis.cache.windows.net:6380,password=" + $redisCacheKey.PrimaryKey +",ssl=True,abortConnect=False"   
$file.AppSettings.RedisCacheConfig = $redisConnectionstring

Write-Host "Update Cosmos DB key values into appSettings.json file"
$file.AppSettings.CosmoDatabaseName = $cosmoDatabaseName
$file.AppSettings.CosmoCollectionName = $cosmoContainerName
$file.AppSettings.CosmoEndpointUrl = "https://" + $cosmoAccountName +".documents.azure.com:443/"

$key = Get-CosmosDbAccountMasterKey -Name $cosmoAccountName -ResourceGroupName $resourceName
$BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($key)            
$PlainKey = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)  
$file.AppSettings.CosmoPrimaryKey = $PlainKey

Write-Host "Update SQL server connection string into appSettings.json file"
$sqlConnectionString = "Server=tcp:"+ $sqlServerName +".database.windows.net,1433;Initial Catalog="+ $sqlDatabaseName +";Persist Security Info=False;User ID="+ $sqlUserName +";Password="+ $sqlPassword +";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
$file.AppSettings.AzureDBConnectionString = $sqlConnectionString

$file | ConvertTo-Json | Out-File -FilePath $filePath -Encoding utf8 -Force    