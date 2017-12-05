using Newtonsoft.Json;
using System;

namespace Agent.Commands
{
    public class CommandEventArgs : EventArgs
    {
        public Command command;
    }

    public abstract class Command
    {
        public string type { get; set; }
        public string sourceIp { get; set; }
        public string tag { get; set; }
        public int sourcePort { get; set; }

        public AgentContact Source { get { return new AgentContact() { ip = sourceIp, port = sourcePort, tag = tag }; } }

        protected Agent Agent { get; private set; }

        public Command()
        {
            type = GetType().Name.ToUpper();
        }

        public Command(Agent agent) : this()
        {
            Agent = agent;
            tag = Agent.TAG;
            sourceIp = Agent.IP;
            sourcePort = Agent.Port;
        }

        public virtual void Inject(Agent agent)
        {
            Agent = agent;
            //SourceIP = Agent.IP;
            //SourcePort = Agent.Port;
        }

        public abstract void ExecuteCommand();
    }
}
