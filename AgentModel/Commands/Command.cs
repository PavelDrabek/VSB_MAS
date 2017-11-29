using AgentModel.Agent;

namespace AgentModel.CommandData
{
    public abstract class Command
    {
        public string CommandName { get { return GetType().Name.ToUpper(); } }

        public string type { get; set; }
        public string sourceIp { get; set; }
        public string tag { get; set; }
        public int sourcePort { get; set; }

        public IAgent Agent { get; private set; }

        public Command()
        {
            type = CommandName;
        }

        public Command(IAgent agent) : this()
        {
            Agent = agent;
            tag = Agent.TAG;
            sourceIp = Agent.IP;
            sourcePort = Agent.Port;
        }

        public void SetFromAgent(IAgent agent)
        {
            Agent = agent;
            tag = Agent.TAG;
            sourceIp = Agent.IP;
            sourcePort = Agent.Port;
        }

        public void Inject(IAgent agent)
        {
            Agent = agent;
            //SourceIP = Agent.IP;
            //SourcePort = Agent.Port;
        }

        public virtual void ExecuteCommand() { }
    }
}
