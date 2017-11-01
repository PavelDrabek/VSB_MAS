
namespace Agent
{
    public class AgentContact
    {
        public string IP { get; private set; }
        public int Port { get; private set; }

        public AgentContact(string ip, int port)
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
