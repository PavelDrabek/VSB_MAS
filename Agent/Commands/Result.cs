using Agent.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class Result : Command
    {
        public string status { get; set; }
        public string value { get; set; }
        public string message { get; set; }

        public Result() : base() { }
        public Result(Agent agent) : base(agent) { }

        public override void ExecuteCommand()
        {
            try {
                var contacts = JsonConvert.DeserializeObject<List<AgentContact>>(value);
                foreach(var contact in contacts) {
                    Agent.AddContact(contact);
                }
            } catch(Exception e) {
                Debug.Log(e.Message, Logger.Level.Error);
            }
        }
    }
}
