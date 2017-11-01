using Agent;
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
        public PackageControl PackageControl { get; private set; }

        public Dictionary<string, Command> Commands { get; private set; }
        public Command[] CommandsArray { get { return new List<Command>(Commands.Values).ToArray(); } }

        public List<Command> ACKWaiting { get; private set; }

        public EventHandler<ReceiveEventArgs> OnAckReceived;


        public Agent(int port)
        {
            Port = port;
            IP = IPHelper.GetLocalIPAddress();

            Commands = new Dictionary<string, Command>();
            ACKWaiting = new List<Command>();

            PackageControl = new PackageControl();

            Receiver = new Receiver(port);
            Receiver.OnReceive += Receiver_OnReceive;
        }

        public void Start()
        {
            Receiver.Start();
            Console.WriteLine("Agent is started on {0}:{1}", IP, Port);

            new Duplicate(this) { ip = IP, port = Port}.Execute();
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

        public void SendOnAckReceived(Ack ack)
        {
            OnAckReceived?.Invoke(this, new ReceiveEventArgs() {
                message = ack.Message,
                sourceIP = ack.SourceIP,
                sourcePort = ack.SourcePort
            });
        }

        private void Receiver_OnReceive(object sender, ReceiveEventArgs e)
        {
            //Console.WriteLine("\n>>Message from {0}:{1}: {2}", e.sourceIP, e.sourcePort, e.message);

            Command c = CommandHandler.GetCommand(CommandsArray, e.message);
            if(c == null) {
                Console.WriteLine("Unknown command " + e.message);
            }
            c.Inject(this);

            if(c is Ack) {
                SendOnAckReceived(c as Ack);
            }
            else {
                Ack ack = new Ack(this) { Message = e.message };
                string message = CommandHandler.CommandToString(ack);
                //Console.WriteLine("\n<<Sending ACK to {0}:{1}: {2}", c.SourceIP, c.SourcePort, message);
                new Sender(null, c.SourceIP, c.SourcePort, message).Send(false);
            }

            c.ExecutedTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            c.Execute();
        }

    }
}
