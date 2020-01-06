using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace PerformanceTestLibrary.Services
{
    public class SystemConfiguration
    {
        static string Executablepath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static int lPathindex = Executablepath.LastIndexOf("\\");
        static string Batchfilepath = Path.Combine(Executablepath.Substring(0, lPathindex), "Scripts\\Dynamic.bat");
        public static string FetchSystemConfiguration()
        {
            string batchfiletext = "powershell.exe " + "\"" + Path.Combine(Executablepath.Substring(0, lPathindex), "Scripts\\SystemConfig.ps1") + "\" " +
               "\"" + Path.Combine(Executablepath.Substring(0, lPathindex), "Scripts\\Systemconfig.txt") + "\"";

            StreamWriter streamWriter = new StreamWriter(Batchfilepath);
            streamWriter.Write(batchfiletext);
            streamWriter.Close();
            System.Diagnostics.Process.Start("\"" + Batchfilepath + "\"").WaitForExit();
            return File.ReadAllText(Path.Combine(Executablepath.Substring(0, lPathindex), "Scripts\\Systemconfig.txt")).ToString();
        }

        public static string FetchConsumedEndpointPingResponse(Dictionary<string, string> parameters)
        {
            string pingrespfile = Path.Combine(Executablepath.Substring(0, lPathindex), "Scripts\\PingResp.txt");

            StringBuilder batchfiletext = new StringBuilder();
            batchfiletext.AppendLine("cd\\");
            batchfiletext.AppendLine(Path.GetPathRoot(Executablepath).TrimEnd('\\'));
            batchfiletext.AppendLine("cd " + Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, lPathindex), "Scripts"));
            batchfiletext.AppendLine("echo. >> " + pingrespfile);
            batchfiletext.AppendLine("echo ^<table^> ^<tr^> ^< td align = \"center\" width = \"100%\" /^> ^</tr^> ^</table^> " + " >> " + pingrespfile);
            batchfiletext.AppendLine("echo Ping response capturing start: >> " + pingrespfile);
            batchfiletext.AppendLine("echo. >> " + pingrespfile);
            batchfiletext.AppendLine("psping " + parameters["PingToRedisWithPort"] + " >> " + pingrespfile);
            batchfiletext.AppendLine("echo. >> " + pingrespfile);
            batchfiletext.AppendLine("psping " + parameters["PingToCosmoWithPort"] + " >> " + pingrespfile);
            batchfiletext.AppendLine("echo. >> " + pingrespfile);
            batchfiletext.AppendLine("psping " + parameters["PingToSqlWithPort"] + " >> " + pingrespfile);
            batchfiletext.AppendLine("echo. >> " + pingrespfile);
            batchfiletext.AppendLine("psping " + parameters["PingToStorageWithPort"] + " >> " + pingrespfile);
            batchfiletext.AppendLine("echo. >> " + pingrespfile);
            batchfiletext.AppendLine("echo Ping response capturing end >> " + pingrespfile);
            WritetoFile(Batchfilepath, batchfiletext.ToString());

            return TextToHtml(File.ReadAllText(pingrespfile).ToString());

        }

        public static string TextToHtml(string text)
        {
            text = HttpUtility.HtmlEncode(text);
            text = text.Replace("\r\n", "\r");
            text = text.Replace("\n", "\r");
            text = text.Replace("\r", "<br>\r\n");
            text = text.Replace("  ", " &nbsp;");
            return text;
        }

        private static void WritetoFile(string batchfilepath, string batchfiletext)
        {
            using (StreamWriter streamWriter = new StreamWriter(batchfilepath))
            {
                streamWriter.Write(batchfiletext);
                streamWriter.Close();
            }
            System.Diagnostics.Process.Start("\"" + batchfilepath + "\"").WaitForExit();

        }


    }
}
