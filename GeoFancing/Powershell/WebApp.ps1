param(
	[Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$resourceName,   
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$locationName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$AppServicePlan,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$webAppName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$source,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$destination    
)

Write-Host "Checking App service plan existence"
$appServicePlan = Get-AzAppServicePlan -Name $AppServicePlan -ResourceGroupName $resourceName -ErrorAction SilentlyContinue
if([string]::IsNullOrEmpty($AppServicePlan.Length))
{
    Write-Host "Creating App Service plan."
    New-AzAppServicePlan -Name $AppServicePlan -Location $locationName -ResourceGroupName $resourceName -Tier Free
    $appServicePlan = Get-AzAppServicePlan -Name $AppServicePlan -ResourceGroupName $resourceName -ErrorAction SilentlyContinue
}

Write-Host "Checking web app existence"
$webApp = Get-AzWebApp -ResourceGroupName $resourceName -Name $webAppName -ErrorAction SilentlyContinue
if([string]::IsNullOrEmpty($webApp))
{
    Write-Host "Creating web app."
    New-AzWebApp -Name $webAppName -Location $locationName -AppServicePlan $AppServicePlan -ResourceGroupName $resourceName 
    $webApp = Get-AzWebApp -ResourceGroupName $resourceName -Name $webAppName
}

Write-Host "Compressing file into zip file"
Compress-Archive -Path $source -Update -DestinationPath $destination -CompressionLevel Optimal

#Publish web app
Write-Host "publishing package on the web app"
Publish-AzWebapp -ResourceGroupName $resourceName -Name $webAppName -ArchivePath $destination
