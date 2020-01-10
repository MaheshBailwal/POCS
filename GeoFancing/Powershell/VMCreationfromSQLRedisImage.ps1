
Login-AzAccount
Set-AzContext -Subscription "1a18fcd7-9120-42f1-a099-c647693079c4" 

Write-Host "Switched to subscription Microsft Azure enterprise"

Write-Host "getdisk"
$disk = Get-AzDisk -ResourceGroupName 'rg-wenco-poc' -DiskName 'WencosqlVM_OsDisk_1_01faaae3dae346ce9d83f0acaa58efb3'

$location = 'EastUS'
$imageName = 'WencosqlredisVM-image-20200109'
$rgName = 'rg-wenco-poc'
$myVMfromImage = 'wencosqlVM' + '7'
$SecurityGroupName = $myVMfromImage + '-nsg'
$VirtualNetworkName ='rg-wenco-poc-vnet'
$SubnetName = 'default'
$PublicIpAddressName = $myVMfromImage + '-ip'
$OpenPorts=3389


Write-Host "imageconfig"
$imageConfig = New-AzImageConfig -Location $location


Write-Host "set imageconfig"
$imageConfig = Set-AzImageOsDisk -Image $imageConfig -OsState Generalized -OsType Windows -ManagedDiskId $disk.Id


Write-Host "new image config"
$image = New-AzImage -ImageName $imageName -ResourceGroupName $rgName -Image $imageConfig

Write-Host "New VM creation from Image"

New-AzVm -ResourceGroupName $rgName -Name $myVMfromImage -Image $image.Id -Location $location -VirtualNetworkName $VirtualNetworkName -SubnetName $SubnetName -SecurityGroupName  $SecurityGroupName -PublicIpAddressName $PublicIpAddressName  -OpenPorts $OpenPorts 

Write-Host "New VM successfuly created from Image"
