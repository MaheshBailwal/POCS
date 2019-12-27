# https://docs.microsoft.com/en-us/powershell/module/az.sql/new-azsqlserver?view=azps-3.2.0
# https://adamtheautomator.com/connect-to-azure-sql-database/
# https://docs.microsoft.com/en-us/powershell/module/sqlserver/invoke-sqlcmd?view=sqlserver-ps

param(
	[Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$resourceName,   
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$locationName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$sqlServerName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$sqlUserName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$sqlPassword,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$sqlDatabaseName,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$sqlServerFirewallRule,
    [Parameter(Mandatory=$True,	ValueFromPipeline=$True)]
	[string]$queryFilePath
)

#$resourceName = "rg-wenco"
#$locationName = "East US"
#
#$sqlServerName = "wenco-server"
#$sqlUserName = "wencosqlserver"
#$sqlPassword = "Wenco#12345"
#$sqlDatabaseName = "wencodb"
#$sqlServerFirewallRule = "wencoFirewallRule"
#$queryFilePath = "F:\AzureTemplates\SqlQuery.sql"

$password = ConvertTo-SecureString $sqlPassword -AsPlainText -Force
$Cred = New-Object System.Management.Automation.PSCredential ($sqlUserName, $password)

# Create a SQL Server
Write-Host "Checking SQL server existence"
$server = Get-AzSqlServer -ResourceGroupName $resourceName -ServerName $sqlServerName -ErrorAction SilentlyContinue
echo $server.ServerName
if([string]::IsNullOrEmpty($server.ServerName))
{
    Write-Host "Creating SQL server"
    New-AzSqlServer -ResourceGroupName $resourceName -Location $locationName -ServerName $sqlServerName -ServerVersion "12.0" -SqlAdministratorCredentials $Cred
    $server = Get-AzSqlServer -ResourceGroupName $resourceName -ServerName $sqlServerName -ErrorAction SilentlyContinue
}

Write-Host "Checking SQL server database existence"
$database = Get-AzSqlDatabase -ResourceGroupName $resourceName -ServerName $sqlServerName -DatabaseName $sqlDatabaseName -ErrorAction SilentlyContinue
echo $database.DatabaseName
if(-not $database)
{
    Write-Host "Creating SQL server database"
    New-AzSqlDatabase -ResourceGroupName $resourceName -ServerName $sqlServerName -DatabaseName $sqlDatabaseName
    $database = Get-AzSqlDatabase -ResourceGroupName $resourceName -ServerName $sqlServerName -DatabaseName $sqlDatabaseName -ErrorAction SilentlyContinue
}

$publicIp = (Invoke-WebRequest ifconfig.me/ip).Content.Trim() -replace "`n"
Write-Host "Checking firewall rule for IP: " $publicIp
$firewallrule = Get-AzSqlServerFirewallRule -ResourceGroupName $resourceName -FirewallRuleName $sqlServerFirewallRule -ServerName $sqlServerName -ErrorAction SilentlyContinue
if(-not $firewallrule)
{    
    Write-Host "Creating firewall rule for IP: " $publicIp    
    New-AzSqlServerFirewallRule -ResourceGroupName $resourceName -FirewallRuleName "wencoFirewallRule" -ServerName $sqlServerName -StartIpAddress $publicIp -EndIpAddress $publicIp
}


$firewallruleForAll = Get-AzSqlServerFirewallRule -ResourceGroupName $resourceName -FirewallRuleName "forallips" -ServerName $sqlServerName -ErrorAction SilentlyContinue
if(-not $firewallruleForAll)
{    
    Write-Host "Creating firewall rule for all IPs"    
    New-AzSqlServerFirewallRule -ResourceGroupName $resourceName -FirewallRuleName "forallips" -ServerName $sqlServerName -StartIpAddress "0.0.0.0" -EndIpAddress "255.255.255.255"
}



#Checking if azure sql module exist
Write-Host "Checking if azure sql module exist"
if(Get-Module Az.Sql)
{
    Write-Host "azure sql module already installed"
}
else
{
    Write-Host "azure sql module installing..."
    Import-Module Az.Sql -Force
}


Write-Host "Invoking sql query on the server"
Invoke-Sqlcmd -ServerInstance $server.FullyQualifiedDomainName -Database $database.DatabaseName -Username $sqlUserName -Password $sqlPassword -InputFile $queryFilePath
