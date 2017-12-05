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

        public AgentContactBook()
        {
            Contacts = new List<AgentContact>();
            Contacts.Add(new AgentContact() { ip = "1", port = 1, tag = "test" });
            Contacts.Add(new AgentContact() { ip = "2", port = 2, tag = "test" });
            Contacts.Add(new AgentContact() { ip = "3", port = 3, tag = "test3" });
        }

        public void Add(string ip, int port, string tag)
        {
            Add(new AgentContact() { ip = ip, port = port, tag = tag });
        }
        public void Add(AgentContact a)
        {
            if(Contains(a.ip, a.port)) {
                return;
            }

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
