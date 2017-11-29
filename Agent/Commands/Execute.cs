using AgentModel.CommandData;
using System.Diagnostics;

namespace Agent.Commands
{
    public class Execute : ExecuteData
    {
        public override void ExecuteCommand()
        {
            ProcessStartInfo ProcessInfo;
            Process Process;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + command);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = true;

            Process = Process.Start(ProcessInfo);
        }
    }
}
