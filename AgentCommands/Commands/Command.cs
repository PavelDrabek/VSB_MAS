namespace Agent.Commands
{
    public abstract class Command
    {
        public string type { get; set; }
        public string sourceIp { get; set; }
        public string tag { get; set; }
        public int sourcePort { get; set; }

        protected IAgent Agent { get; private set; }

        public Command()
        {
            type = GetType().Name.ToUpper();
        }

        public Command(IAgent agent) : this()
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
