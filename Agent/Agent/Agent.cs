using Agent.Commands;
using Agent.Communication;
using Agent.Utilities;
using System;
using System.Collections.Generic;

namespace Agent
{
    public class Agent
    {
        public string IP { get; private set; }
        public int Port { get; private set; }

        public Receiver Receiver { get; private set; }

        public Dictionary<string, Command> Commands { get; private set; }
        public Command[] CommandsArray { get { return new List<Command>(Commands.Values).ToArray(); } }

        public List<Command> ACKWaiting { get; private set; }

        public Agent(int port)
        {
            Port = port;
            IP = IPHelper.GetLocalIPAddress();

            Commands = new Dictionary<string, Command>();
            ACKWaiting = new List<Command>();

            Receiver = new Receiver(port);
            Receiver.OnReceive += Receiver_OnReceive;
        }

        public void Start()
        {
            Receiver.Start();
            Console.WriteLine("Agent is started on {0}:{1}", IP, Port);
        }

        public void Stop()
        {
            Receiver.Stop();
            Console.WriteLine("Agent is stopped");
        }

        public void AddCommand(Command command)
        {
            Commands[command.GetType().Name.ToLower()] = command;
            Console.WriteLine("Adding command {0}", command.GetType().Name);
        }

        private void Receiver_OnReceive(object sender, ReceiveEventArgs e)
        {
            Console.WriteLine("{0}:{1} >> {2}", e.fromIP, e.fromPort, e.message);

            Command c = CommandHandler.GetCommand(CommandsArray, e.message);
            if(c == null) {
                Console.WriteLine("Unknown command " + e.message);
            }
            c.Inject(this);
            c.ExecutedTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            c.Execute();
        }

    }
}
