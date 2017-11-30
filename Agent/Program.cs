using Agent.Commands;

namespace Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            //JArray data = JsonConvert.DeserializeObject<JArray>(@"[ ""send"" ,""10.0.0.1:88"", [ ""send"", ""10.0.0.1:90"", [""store"", ""ahoj""] ] ]");
            //string content = data[2].ToString();

            //JObject dataObj = JsonConvert.DeserializeObject<JObject>(@"{""type"":""send"",""receiver"":""192.168.0.1:88"",""content"":{""type"":""send"",""receiver"":""192.168.0.1:88"",""content"":{""type"":""store"",""value"":""ahoj""}}");
            //string content2 = dataObj["content"].ToString();


            Agent agent = new Agent();

            agent.AddCommand(new Send());
            agent.AddCommand(new Ack());
            agent.AddCommand(new Halt());
            agent.AddCommand(new Store());
            agent.AddCommand(new Duplicate());
            agent.AddCommand(new Package());
            agent.AddCommand(new PackageReceived());
            agent.AddCommand(new Execute());

            agent.Start();
        }
    }
}
