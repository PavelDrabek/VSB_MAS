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
        public JObject message { get; set; }

        public void Call(string ip, int port, string message)
        {
            this.ip = ip;
            this.port = port;
            this.message = JsonConvert.DeserializeObject<JObject>(message);
            Execute();
        }

        public override void Execute()
        {
            string msg = message.ToString(Formatting.None);
            Console.WriteLine("Sending \"{0}\" to {1}:{2}", msg, ip, port);
            new Sender(Agent, ip, port, msg).Send();
        }
    }
}
