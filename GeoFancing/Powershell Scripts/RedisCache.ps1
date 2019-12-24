
param(
	[Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$resourceName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$redisCacheName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$locationName
)

Write-Host "Checking Redis cache existence"
$redisCache = Get-AzRedisCache -ResourceGroupName $resourceName -Name $redisCacheName -ErrorAction SilentlyContinue
echo $redisCache
if([string]::IsNullOrEmpty($redisCache.Name))
{
    Write-Host "Creating Redis cache."
    New-AzRedisCache -ResourceGroupName $resourceName -Name $redisCacheName -Location $locationName -Size 1GB -Sku Standard -EnableNonSslPort false
    $redisCache = Get-AzRedisCache -ResourceGroupName $resourceName -Name $redisCacheName -ErrorAction SilentlyContinue    
}