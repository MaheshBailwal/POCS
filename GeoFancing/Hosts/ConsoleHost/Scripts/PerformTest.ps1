$filepath=[string]$args[0]
Clear-Content $filepath;
Add-Content $filepath ('Name:' + [string](Get-WmiObject win32_processor).Name);
Add-Content $filepath (systeminfo | Select-String 'Total Physical Memory:');
Add-Content $filepath ('NumberOfCores:' + [string](Get-WmiObject win32_processor).NumberOfCores);