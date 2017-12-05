using Agent.Communication;
using Newtonsoft.Json;

namespace Agent.Commands
{
    public class Agents : Command
    {
        public override void ExecuteCommand()
        {
            var contacts = Agent.AgentContactBook.Contacts;

            Result result = new Result(Agent);
            result.status = "success";
            result.value = JsonConvert.SerializeObject(contacts);
            result.message = "";

            new Sender(null, sourceIp, sourcePort, result).Send(false); 
        }
    }
}
