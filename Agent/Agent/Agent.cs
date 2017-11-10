using Agent;
using Agent.Commands;
using Agent.Communication;
using Agent.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Agent
{
    public class Agent
    {
        public string TAG { get { return "DRA0042"; } }
        public string IP { get; private set; }
        public int Port { get; private set; }

        public Receiver Receiver { get; private set; }
        public PackageControl PackageControl { get; private set; }

        public Dictionary<string, Command> Commands { get; private set; }
        public Command[] CommandsArray { get { return new List<Command>(Commands.Values).ToArray(); } }

        public List<Command> ACKWaiting { get; private set; }

        public EventHandler<ReceiveEventArgs> OnAckReceived;
        public List<IAckReceiver> ackReceivers;

        public EventHandler<EventArgs> OnSecondTick;
        private Thread tickThread;

        public Agent(int port)
        {
            Port = port;
            IP = IPHelper.GetLocalIPAddress();

            Commands = new Dictionary<string, Command>();
            ACKWaiting = new List<Command>();

            PackageControl = new PackageControl();
            ackReceivers = new List<IAckReceiver>();

            Receiver = new Receiver(port);
            Receiver.OnReceive += Receiver_OnReceive;

            tickThread = new Thread(TicTac);
            tickThread.Start();
        }

        private void TicTac()
        {
            for(;;) {
                Thread.Sleep(200);
                //Console.WriteLine("Tick");
                OnSecondTick?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Start()
        {
            Receiver.Start();
            Console.WriteLine("Agent is started on {0}:{1}", IP, Port);

            //new Duplicate(this) { ip = "192.168.43.56", port = 58536 }.Execute();
            //new Duplicate(this) { ip = "192.168.43.125", port = 53000 }.Execute();
            new Duplicate(this) { ip = IP, port = Port }.Execute();
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

        static int x = 0;
        public bool SendOnAckReceived(Ack ack)
        {
            string message = ack.Message;
            for(int i = 0; i < ackReceivers.Count; i++) {
                if(ackReceivers[i].ReceiveACK(ack.sourceIp, ack.sourcePort, message)) {
                    ackReceivers.RemoveAt(i);
                    i--;
                    return true;
                }
            }
            //Console.WriteLine("ACK noone");
            return false;

            //OnAckReceived?.Invoke(this, new ReceiveEventArgs() {
            //    message = ack.Message,
            //    sourceIP = ack.SourceIP,
            //    sourcePort = ack.SourcePort
            //});
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
                bool received = SendOnAckReceived(c as Ack);
                if(received) {
                    //Console.WriteLine("Agent ack received {0}", x++);
                } else {
                    Console.WriteLine("Agent ack error {0}", x++);
                    //Console.WriteLine("Agent ack error {0} {1}", x++, e.message);
                }
            }
            else {
                //Console.WriteLine("Agent not ack {0}", x++);

                //JObject json = JsonConvert.DeserializeObject<JObject>(e.message);
                Ack ack = new Ack(this) { Message = e.message };
                string message = CommandHandler.CommandToString(ack);
                //Console.WriteLine("\n<<Sending ACK to {0}:{1}: {2}", c.SourceIP, c.SourcePort, message);
                new Sender(null, c.sourceIp, c.sourcePort, message).Send(false);
            }

            c.ExecutedTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            c.Execute();
        }

    }
}
