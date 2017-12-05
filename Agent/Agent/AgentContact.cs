
namespace Agent
{
    public class AgentContact
    {
        public string ip { get; set; }
        public int port { get; set; }
        public string tag { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1} [{2}]", ip, port, tag);
        }
    }
}
