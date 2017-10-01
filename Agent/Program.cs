using Agent.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            JArray data = JsonConvert.DeserializeObject<JArray>(@"[ ""send"" ,""10.0.0.1:88"", [ ""send"", ""10.0.0.1:90"", [""store"", ""ahoj""] ] ]");
            string content = data[2].ToString();

            JObject dataObj = JsonConvert.DeserializeObject<JObject>(@"{""type"":""send"",""receiver"":""192.168.0.1:88"",""content"":{""type"":""send"",""receiver"":""192.168.0.1:88"",""content"":{""type"":""store"",""value"":""ahoj""}}");
            string content2 = dataObj["content"].ToString();


            Agent agent = new Agent(8888);

            agent.AddCommand(new Send());

            agent.Start();
        }
    }
}
