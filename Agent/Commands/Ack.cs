﻿
using Agent.Communication;
using Newtonsoft.Json.Linq;

namespace Agent.Commands
{
    public class Ack : Command
    {
        public string message { get; set; }

        public Ack() : base() { }

        public Ack(Agent agent) : base(agent) { }

        public override void ExecuteCommand()
        {
            // TODO
            // A) Ma posilat ACK
            // B) Ma ukoncit command cekani na ACK - asi toto
        }
    }
}
