using Agent.Communication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Agent.Commands
{
    public class Send : Command
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public JObject Message { get; set; }

        public void Call(string ip, int port, string message)
        {
            IP = ip;
            Port = port;
            Message = JsonConvert.DeserializeObject<JObject>(message);
            Execute();
        }
        public override void Execute()
        {
            string msg = Message.ToString(Formatting.None);
            Console.WriteLine("Sending \"{0}\" to {1}:{2}", msg, IP, Port);
            new Sender(Agent.Receiver, IP, Port, msg).Send();
        }
    }
}
