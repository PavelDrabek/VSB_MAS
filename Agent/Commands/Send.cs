using Agent.Communication;
using AgentModel.CommandData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Agent.Commands
{
    public class Send : SendData
    {
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
