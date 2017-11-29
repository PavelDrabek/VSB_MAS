using AgentModel.CommandData;

namespace Agent.Commands
{
    public class Halt : HaltData
    {
        public override void ExecuteCommand()
        {
            Agent.Stop();
        }
    }
}
