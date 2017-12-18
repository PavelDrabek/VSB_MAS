using Agent.Utilities;
using System;
using System.Collections.Generic;

namespace Agent
{
    public class AgentContactBook
    {
        public class ContactEventArgs : EventArgs
        {
            public AgentContact contact;
        }

        public List<AgentContact> Contacts { get; private set; }

        public EventHandler<ContactEventArgs> OnContactAdded;
        public EventHandler<ContactEventArgs> OnContactRemved;

        private Agent agent;

        public AgentContactBook(Agent a)
        {
            agent = a;
            Contacts = new List<AgentContact>();
            //Contacts.Add(new AgentContact() { ip = "1", port = 1, tag = "test" });
            //Contacts.Add(new AgentContact() { ip = "2", port = 2, tag = "test" });
            //Contacts.Add(new AgentContact() { ip = "3", port = 3, tag = "test3" });
        }

        public void Add(string ip, int port, string tag)
        {
            Add(new AgentContact() { ip = ip, port = port, tag = tag });
        }
        public void Add(AgentContact a)
        {
            if(a.ip.Equals(agent.IP) && a.port == agent.Port) {
                return;
            }
            if(a.tag.Equals("Logger") || a.tag.Equals("Initializer")) {
                return;
            }

            if(Contains(a.ip, a.port)) {
                return;
            }

            Debug.Log(string.Format("Adding contact: {0}", a.ToString()), Logger.Level.Command);
            Contacts.Add(a);
            OnContactAdded?.Invoke(this, new ContactEventArgs() { contact = a });
        }

        public bool Remove(AgentContact a)
        {
             return Remove(a.ip, a.port);
        }

        public bool Remove(string ip, int port)
        {
            bool removed = Contacts.RemoveAll(x => x.ip.Equals(ip) && x.port == port) > 0;
            Debug.Log(string.Format("Removing contact: {0}:{1}", ip, port), Logger.Level.Command);

            if(removed) {
                OnContactRemved?.Invoke(this, new ContactEventArgs() { contact = new AgentContact() { ip = ip, port = port } });
            }

            return removed;
        }

        public bool Contains(string ip, int port)
        {
            return Contacts.Exists(x => x.ip.Equals(ip) && x.port == port);
        }
    }
}
