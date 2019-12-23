Install-Module -Name CosmosDB

# https://github.com/PlagueHO/CosmosDB/wiki/New-CosmosDbDocument
# https://github.com/PlagueHO/CosmosDB - Converting secure key
$resourceName = "rg-wenco"
$accountName = "wenco-cosmo-testdb" # must be lowercase and < 31 characters .
$databaseName = "wenco-cosmoDB"
$containerName = "wenco-cosmoDB-container"
$partitionkey = "/Site"

#$cosmosDbContext = New-CosmosDbContext -Account $accountName -Database $databaseName -ResourceGroup $resourceName -MasterKeyType PrimaryMasterKey


#New-CosmosDbCollection -Context $cosmosDbContext -Id $containerName -PartitionKey 'account' -OfferThroughput 50000

#0..9 | Foreach-Object {
#    $document = @"
#{
#    `"id`": `"$([Guid]::NewGuid().ToString())`",
#    `"content`": `"Some string`",
#    `"more`": `"Some other string`"
#}
#"@
#New-CosmosDbDocument -Context $cosmosDbContext -CollectionId $containerName -DocumentBody $document -PartitionKey 'account'
#}

# Remove-CosmosDbAccount -Name $accountName -ResourceGroupName $resourceName -Force

New-CosmosDbAccount -Name $accountName -ResourceGroupName $resourceName -Location 'East US'

$key = Get-CosmosDbAccountMasterKey $accountName -ResourceGroupName $resourceName

$primaryKey = ConvertTo-SecureString -String $key -AsPlainText -Force

$cosmosDbContext = New-CosmosDbContext -Account $accountName -Database $databaseName -Key $primaryKey