using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PerformanceTestLibrary.Services
{
    public class SystemConfiguration
    {
       public static string  FetchSystemConfiguration()
        {
            int lPathindex = System.Reflection.Assembly.GetExecutingAssembly().Location.LastIndexOf("\\");

            string batchfilepath = Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, lPathindex), "Scripts\\Executepowershell.bat");
            string ss = "powershell.exe " + "\"" + Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, lPathindex), "Scripts\\PerformTest.ps1") + "\" " +
               "\"" + Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, lPathindex), "Scripts\\Systemconfig.txt") + "\"";

            StreamWriter streamWriter = new StreamWriter(batchfilepath);
            streamWriter.Write(ss);
            streamWriter.Close();
            System.Diagnostics.Process.Start("\"" + batchfilepath + "\"").WaitForExit();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("********************************************************************");
            Console.WriteLine("Performance Test executed on below system configurations: ");
            var txt =           File.ReadAllText(Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, lPathindex), "Scripts\\Systemconfig.txt")).ToString();
            Console.WriteLine(File.ReadAllText(Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, lPathindex), "Scripts\\Systemconfig.txt")).ToString()); ;
            Console.WriteLine("********************************************************************");
            Console.ResetColor();
            return txt;
        }
    }
}
