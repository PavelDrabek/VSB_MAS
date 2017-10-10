
namespace Agent.Commands
{
    public class Ack : Command
    {
        public override bool NeedsACK { get { return false; } } 

        public override void Execute()
        {
            //Agent.ACKWaiting
        }
    }
}
