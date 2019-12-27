$filepath=[string]$args[0]
Clear-Content $filepath;
Add-Content $filepath("<table border=1><tr><b><td>Sytem Configuration</td><tr>");
Add-Content $filepath("<tr><td>Sytem Configuration</td><tr>");

Add-Content $filepath ('<tr><td>Name:' + [string](Get-WmiObject win32_processor).Name + '</td></tr>');
Add-Content $filepath ('<tr><td>Total Physical Memory:' +(systeminfo | Select-String 'Total Physical Memory:').ToString().Split(':')[1].Trim()+ '</td></tr>');
Add-Content $filepath ('<tr><td>NumberOfCores:' + [string](Get-WmiObject win32_processor).NumberOfCores+ '</td></tr>');
Add-Content $filepath("</table>");
