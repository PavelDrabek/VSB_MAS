
using Agent.Communication;
using Newtonsoft.Json.Linq;

namespace Agent.Commands
{
    public class Ack : Command
    {
        public string Message { get; set; }

        public Ack() : base() { }

        public Ack(Agent agent) : base(agent) { }

        public override void Execute()
        {
            // TODO
            // A) Ma posilat ACK
            // B) Ma ukoncit command cekani na ACK - asi toto
        }
    }
}
