using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class Halt : Command
    {
        public override void Execute()
        {
            Agent.Stop();
        }
    }
}
