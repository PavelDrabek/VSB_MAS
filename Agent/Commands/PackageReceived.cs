using AgentModel.CommandData;
using System;

namespace Agent.Commands
{
    public class PackageReceived : PackageReceivedData
    {
        public override void ExecuteCommand()
        {
            Console.WriteLine("Package Received from {0}:{1}", sourceIp, sourcePort);
        }
    }
}
