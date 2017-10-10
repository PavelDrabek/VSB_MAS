using Newtonsoft.Json;

namespace Agent.Commands
{
    public abstract class Command
    {
        public string Type { get; set; }
        public virtual bool NeedsACK { get { return true; } }
        [JsonIgnore]
        public long ExecutedTime { get; set; }

        protected Agent Agent { get; private set; }

        public Command()
        {
            Type = GetType().Name;
        }

        public void Inject(Agent agent)
        {
            Agent = agent;
        }

        public abstract void Execute();
    }
}
