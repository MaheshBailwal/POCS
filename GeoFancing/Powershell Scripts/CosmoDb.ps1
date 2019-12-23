# https://docs.microsoft.com/en-us/azure/cosmos-db/manage-with-powershell
# Create an Azure Cosmos Account for Core (SQL) API
$resourceName = "rg-wenco"
$locationName = "East US"
$accountName = "wenco-cosmo-testdb" # must be lowercase and < 31 characters .
$databaseName = "wenco-cosmoDB"
$containerName = "wenco-cosmoDB-container"
$DBPath = $accountName + "/sql/" + $databaseName
$ConatinerPath = $accountName + "/sql/" + $databaseName + "/" + $containerName

$locations = @(
    @{ "locationName"="West US"; "failoverPriority"=0 },
    @{ "locationName"="East US"; "failoverPriority"=1 }
)

$consistencyPolicy = @{
    "defaultConsistencyLevel"="Session";
    "maxIntervalInSeconds"=5;
    "maxStalenessPrefix"=100
}

$CosmosDBProperties = @{
    "databaseAccountOfferType"="Standard";
    "locations"=$locations;
    "consistencyPolicy"=$consistencyPolicy;
    "enableMultipleWriteLocations"="false"
}

$cosmoDb = Get-AzResource -ResourceGroupName $resourceName -Name $accountName | Select-Object Properties

Write-Host "Checking cosmosDB database existence"
if($cosmoDb)
{
    Write-Host "Creating CosmosDB Account"
    # Create an azure cosmo account
    # New-AzResource -ResourceType "Microsoft.DocumentDb/databaseAccounts" `
    # -ApiVersion "2015-04-08" -ResourceGroupName $resourceName -Location $locationName `
    # -Name $accountName -PropertyObject $CosmosDBProperties -Force

    Write-Host "Creating CosmosDB Database"

    #Create a database
    $DataBaseProperties = @{
        "resource"=@{"id"= $databaseName}
    }

    #New-AzResource -ResourceType "Microsoft.DocumentDb/databaseAccounts/apis/databases" -ApiVersion "2015-04-08" -ResourceGroupName $resourceName -Name $DBPath -PropertyObject $DataBaseProperties -Force

    Write-Host "Creating CosmosDB Container"

    # Creating cosmos Db container
    $ContainerProperties = @{
    "resource"=@{
        "id"=$containerName;
        "partitionKey"=@{
            "paths"=@("/Site");
            "kind"="Hash"
        }
    };
    "options"=@{ "Throughput"="400" }
    }

    New-AzResource -ResourceType "Microsoft.DocumentDb/databaseAccounts/apis/databases/containers" -ApiVersion "2015-04-08" -ResourceGroupName $resourceName -Name $ConatinerPath -PropertyObject $ContainerProperties -Force
    
}
