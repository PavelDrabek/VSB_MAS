
namespace Agent
{
    public class AgentContact
    {
        public string IP { get; set; }
        public int Port { get; set; }

        public override string ToString()
        {
            return IP + ":" + Port;
        }
    }
}
