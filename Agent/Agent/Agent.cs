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
        public string TAG { get; set; }
        public string IP { get; private set; }
        public int Port { get; private set; }

        public Receiver Receiver { get; private set; }
        public PackageControl PackageControl { get; private set; }

        public Dictionary<string, Command> Commands { get; private set; }
        public Command[] CommandsArray { get { return new List<Command>(Commands.Values).ToArray(); } }

        public EventHandler<ReceiveEventArgs> OnAckReceived;
        public List<IAckReceiver> ackReceivers;

        public EventHandler<EventArgs> OnSecondTick;
        private Thread tickThread;

        private ConfigData config;

        public Agent()
        {
            LoadConfig("config.xml");
            IP = IPHelper.GetLocalIPAddress();

            Commands = new Dictionary<string, Command>();

            PackageControl = new PackageControl();
            ackReceivers = new List<IAckReceiver>();

            Receiver = new Receiver(Port);
            Receiver.OnReceive += Receiver_OnReceive;

            tickThread = new Thread(TicTac);
            tickThread.Start();
        }

        private void TicTac()
        {
            for(;;) {
                Thread.Sleep(500);
                //Console.WriteLine("Tick");
                OnSecondTick?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Start()
        {
            Receiver.Start();
            Console.WriteLine("Agent is started on {0}:{1}", IP, Port);

            if(!string.IsNullOrEmpty(config.StartCommand)) {
                Console.WriteLine("Executing command {0}", config.StartCommand);
                Command c = CommandHandler.GetCommand(CommandsArray, config.StartCommand);
                c.Inject(this);
                c.Execute();
            }

            string ip = "192.168.43.125";
            int port = 11111;

            //var d = new Duplicate(this) { ip = IP, port = Port };

            var d = new Duplicate(this) { ip = ip, port = port };
            var s = new Send() {
                ip = ip,
                port = port,
                message = CommandHandler.CommandToString(new Send() {
                    ip = IP,
                    port = Port,
                    message = CommandHandler.CommandToString(new Duplicate() { ip = ip, port = port })
                })
            };
            s.Inject(this);
            //s.Execute();
            //new Duplicate(this) { ip = "192.168.43.125", port = 53000 }.Execute();
            //new Duplicate(this) { ip = IP, port = Port }.Execute();
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
            string message = ack.message;
            for(int i = 0; i < ackReceivers.Count; i++) {
                if(ackReceivers[i].ReceiveACK(ack.sourceIp, ack.sourcePort, message)) {
                    ackReceivers.RemoveAt(i);
                    i--;
                    return true;
                }
            }
            return false;
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
                    //Console.WriteLine("Agent ack received {0}", e.message);
                } else {
                    //Console.WriteLine("Agent received error {0}", e.message);
                    //Console.WriteLine("Agent ack error {0}", x++);
                    //Console.WriteLine("Agent ack error {0} {1}", x++, e.message);
                }
            }
            else {
                //Console.WriteLine("Agent not ack {0}", x++);

                //JObject json = JsonConvert.DeserializeObject<JObject>(e.message);
                Ack ack = new Ack(this) { message = e.message };
                string message = CommandHandler.CommandToString(ack);
                //Console.WriteLine("\n<<Sending ACK to {0}:{1}: {2}", c.SourceIP, c.SourcePort, message);
                //Console.WriteLine("Sending ack {0}", message);
                new Sender(null, c.sourceIp, c.sourcePort, message).Send(false);
            }

            c.ExecutedTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            c.Execute();
        }


        private void LoadConfig(string config_path)
        {
            config = ConfigData.Deserialize(config_path);

            Port = (config.Port != 0) ? config.Port : new Random().Next(config.PortFrom, config.PortTo);
            TAG = config.Tag;
        }
    }
}
