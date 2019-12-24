
param(
	[Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$resourceName,   
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$locationName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$accountName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$databaseName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$containerName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$partitionkey
)

#Checking if cosmo db module exist
Write-Host "Checking if cosmo db module exist"
if(Get-Module cosmosdb)
{
    Write-Host "Cosmo db module already installed"
}
else
{
    Write-Host "Cosmo db module installing..."
    Install-Module -Name CosmosDB
}


# https://github.com/PlagueHO/CosmosDB/wiki/New-CosmosDbDocument
# https://github.com/PlagueHO/CosmosDB - Converting secure key

# https://docs.microsoft.com/en-us/azure/cosmos-db/manage-with-powershell
# Create an Azure Cosmos Account for Core (SQL) API
#$resourceName = "rg-wenco"
#$locationName = "East US"
#
#$accountName = "wenco-cosmo-testdb" # must be lowercase and < 31 characters .
#$databaseName = "wenco-cosmoDB"
#$containerName = "wenco-cosmoDB-container"
#$partitionkey = "site"

$cosmoDb = Get-AzResource -ResourceGroupName $resourceName -Name $accountName
Write-Host "Checking cosmos DB account existence"
if(-not $cosmoDb)
{  
    # Create an azure cosmos account
    Write-Host "Creating Cosmos DB Account"
    New-CosmosDbAccount -Name $accountName -ResourceGroupName $resourceName -Location $locationName    
    $cosmoDb = Get-AzResource -ResourceGroupName $resourceName -Name $accountName
}

#Create a db context
    Write-Host "Creating Cosmos DB context"
    $key = Get-CosmosDbAccountMasterKey -Name $accountName -ResourceGroupName $resourceName
    $cosmosDbContext = New-CosmosDbContext -Account $accountName -Database $databaseName -Key $key

Write-Host "Checking Cosmos DB database existence"
$cosmodatabase = Get-CosmosDbDatabase -Context $cosmosDbContext
if(-not $cosmodatabase)
{
    #Create a database
    Write-Host "Creating Cosmos DB Database"    
    New-CosmosDbDatabase -Context $cosmosDbContext -Id $databaseName
    $cosmodatabase = Get-CosmosDbDatabase -Context $cosmosDbContext
}

Write-Host "Checking Cosmos DB container"
$cosmosContainer = Get-CosmosDbCollection -Context $cosmosDbContext -Database $databaseName
if(-not $cosmosContainer)
{
    #Create a container
    Write-Host "Creating CosmosDB Container"
    New-CosmosDbCollection -Context $cosmosDbContext -Id $containerName -PartitionKey $partitionkey -OfferThroughput 400
    $cosmosContainer = Get-CosmosDbCollection -Context $cosmosDbContext -Database $cosmodatabase
 }



#$file = ([System.IO.File]::ReadAllText($filePath)  | ConvertFrom-Json)
#Write-Host "Inserting New Documents"
#
#for($i =1 ; $i -lt 11; $i ++){    
#    
#    $file.id = "$([Guid]::NewGuid().ToString())";
#    $file.SiteID = $i;
#    $file.Name = "Site" + $i;    
#    #Write-Host $file
#    $fileJson = $file | ConvertTo-Json -Depth 6
#    #Write-Host $fileJson
#    
#    New-CosmosDbDocument -Context $cosmosDbContext -CollectionId $containerName -DocumentBody $fileJson -PartitionKey "AAA" -ErrorAction Inquire
#}



