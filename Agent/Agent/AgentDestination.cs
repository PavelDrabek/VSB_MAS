
namespace Agent
{
    public class AgentDestination
    {
        public string IP { get; private set; }
        public int Port { get; private set; }

        public AgentDestination(string ip, int port)
        {
            IP = ip;
            Port = port;
        }

        public override string ToString()
        {
            return IP + ":" + Port;
        }
    }
}
