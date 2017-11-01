using Newtonsoft.Json;

namespace Agent.Commands
{
    public abstract class Command
    {
        public string Type { get; set; }
        public string SourceIP { get; set; }
        public int SourcePort { get; set; }
        [JsonIgnore]
        public long ExecutedTime { get; set; }

        protected Agent Agent { get; private set; }

        public Command()
        {
            Type = GetType().Name;
        }

        public Command(Agent agent) : this()
        {
            Agent = agent;
            SourceIP = Agent.IP;
            SourcePort = Agent.Port;
        }

        public void Inject(Agent agent)
        {
            Agent = agent;
            //SourceIP = Agent.IP;
            //SourcePort = Agent.Port;
        }

        public abstract void Execute();
    }
}
