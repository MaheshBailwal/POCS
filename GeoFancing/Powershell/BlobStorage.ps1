param(
	[Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$resourceName,   
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$locationName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$blobStorageName    
)

Write-Host "Check blob storage existence"
$storage = Get-AzStorageAccount -ResourceGroupName $resourceName -Name $blobStorageName -ErrorAction SilentlyContinue
if(-not $storage)
{
    Write-Host "Creating blob storage...."
    New-AzStorageAccount -ResourceGroupName $resourceName -Location $locationName -Name $blobStorageName -Kind Storage -SkuName Standard_LRS -ErrorAction Stop
}