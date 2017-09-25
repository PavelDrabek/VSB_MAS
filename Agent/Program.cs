using Agent.Commands;
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
            Agent agent = new Agent(8888);

            agent.AddCommand(new Send());

            agent.Execute();
        }
    }
}
