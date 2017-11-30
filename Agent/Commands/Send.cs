using Agent.Communication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Agent.Commands
{
    public class Send : Command
    {
        public string ip { get; set; }
        public int port { get; set; }
        public string message { get; set; }

        public Send() : base() { }
        public Send(Agent agent) : base(agent) { }

        public void Call(string ip, int port, string message)
        {
            this.ip = ip;
            this.port = port;
            //this.message = JsonConvert.DeserializeObject<JObject>(message);
            this.message = message;
            ExecuteCommand();
        }

        public override void ExecuteCommand()
        {
            var jobject = JsonConvert.DeserializeObject<JObject>(message);
            jobject["sourceIp"] = Agent.IP;
            jobject["sourcePort"] = Agent.Port;
            jobject["tag"] = Agent.TAG;
            string m = JsonConvert.SerializeObject(jobject);

            //string msg = message.ToString(Formatting.None);
            //Console.WriteLine("Sending \"{0}\" to {1}:{2}", msg, ip, port);
            new Sender(Agent, ip, port, m).Send();
        }
    }
}
