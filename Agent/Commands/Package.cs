using Agent.Communication;
using AgentModel.CommandData;

namespace Agent.Commands
{
    public class Package : PackageData
    {
        public override void ExecuteCommand()
        {
            bool result = (Agent as Agent).PackageControl.Add(fileName, data, order, partsCount, sourceIp + "_" + sourcePort + "/");
            if(result) {
                var c = new PackageReceived();
                c.SetFromAgent(Agent);
                new Sender(Agent, sourceIp, sourcePort, c);
            }
        }
    }
}
