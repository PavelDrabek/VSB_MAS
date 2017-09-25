using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class Command
    {
        protected Agent Agent { get; private set; }

        public void Inject(Agent agent)
        {
            Agent = agent;
        }
    }
}
