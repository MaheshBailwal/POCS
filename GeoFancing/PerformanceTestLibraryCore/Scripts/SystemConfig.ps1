$filepath=[string]$args[0]
Clear-Content $filepath;
Add-Content $filepath("<table><tr><b><td></td><tr>");
Add-Content $filepath ('<tr><td>Name:' + [string](Get-WmiObject win32_processor).Name + '');
Add-Content $filepath (' Total Physical Memory:' +(systeminfo | Select-String 'Total Physical Memory:').ToString().Split(':')[1].Trim()+ '');
Add-Content $filepath (' NumberOfCores:' + [string](Get-WmiObject win32_processor).NumberOfCores+ '</td></tr>');
Add-Content $filepath("</table>");
