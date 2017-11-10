using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class Package : Command
    {
        public string data { get; set; }
        public int order { get; set; }
        public string fileName { get; set; }
        public int partsCount { get; set; }

        public Package() : base() { }
        public Package(Agent agent) : base(agent) { }

        public override void Execute()
        {
            Agent.PackageControl.Add(fileName, data, order, partsCount);
        }
    }
}
