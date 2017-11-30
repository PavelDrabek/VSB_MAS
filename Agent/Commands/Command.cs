using Newtonsoft.Json;

namespace Agent.Commands
{
    public abstract class Command
    {
        public string type { get; set; }
        public string sourceIp { get; set; }
        public string tag { get; set; }
        public int sourcePort { get; set; }
        [JsonIgnore]
        public long ExecutedTime { get; set; }

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

        public void Inject(Agent agent)
        {
            Agent = agent;
            //SourceIP = Agent.IP;
            //SourcePort = Agent.Port;
        }

        public abstract void ExecuteCommand();
    }
}
