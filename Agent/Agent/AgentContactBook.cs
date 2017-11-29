using System.Collections.Generic;

namespace Agent
{
    public class AgentContactBook
    {
        public List<AgentContact> Contacts { get; private set; }

        public AgentContactBook()
        {
            Contacts = new List<AgentContact>();
        }

        public void Add(string ip, int port)
        {
            Add(new AgentContact() { IP = ip, Port = port });
        }
        public void Add(AgentContact a)
        {
            Contacts.Add(a);
        }

        public bool Remove(AgentContact a)
        {
            return Contacts.Remove(a);
        }

        public bool Remove(string ip, int port)
        {
            return Contacts.RemoveAll(x => x.IP.Equals(ip) && x.Port == port) > 0;
        }
    }
}
