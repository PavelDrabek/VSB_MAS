using Agent.Communication;
using System;
using System.IO;
using System.Threading;

namespace Agent.Commands
{
    public class PackageReceived : Command
    {
        public PackageReceived()
        {
        }

        public PackageReceived(Agent agent) : base(agent)
        {
        }

        public override void Execute()
        {
            Console.WriteLine("Package Received from {0}:{1}", sourceIp, sourcePort);
        }
    }
}
