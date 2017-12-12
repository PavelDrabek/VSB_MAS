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

        public AgentContact Contact { get { return new AgentContact() { ip = IP, port = Port, tag = TAG }; } }

        public Receiver Receiver { get; private set; }
        public PackageControl PackageControl { get; private set; }
        public AgentContactBook AgentContactBook { get; set; }

        public Dictionary<string, Command> Commands { get; private set; }
        public Command[] CommandsArray { get { return new List<Command>(Commands.Values).ToArray(); } }

        public EventHandler<CommandEventArgs> OnAckReceived;
        public EventHandler<CommandEventArgs> OnCommandReceived;
        public List<IAckReceiver> ackReceivers;
        public List<IResultListener> resultListeners;

        public Dictionary<string, Command> PendingCommands { get; private set; }

        public EventHandler<EventArgs> OnSecondTick;
        private Thread tickThread;

        public ConfigData Config { get; private set; }

        public Agent()
        {
            AgentContactBook = new AgentContactBook(this);

            LoadConfig("config.xml");
            IP = IPHelper.GetLocalIPAddress();

            Commands = new Dictionary<string, Command>();
            PendingCommands = new Dictionary<string, Command>();

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
            new Sender(this, Config.LoggerContact, new Store(this) { value = string.Format("START {0}:{1} {2}", IP, Port, TAG) }).Send(false);

            if(!string.IsNullOrEmpty(Config.StartCommand)) {
                Console.WriteLine("Executing command {0}", Config.StartCommand);
                Command c = CommandHandler.GetCommand(CommandsArray, Config.StartCommand);
                c.Inject(this);
                Debug.Log(string.Format("Start Command executing - {0}", c.type), Logger.Level.Command);
                c.ExecuteCommand();
            }

            //string ip = "192.168.43.56";
            string ip = "192.168.43.125";
            int port = 11111;

            //var d = new Duplicate(this) { ip = IP, port = Port };

            var d = new Send() {
                ip = IP,
                port = Port,
                message = CommandHandler.CommandToString(new Duplicate() { ip = ip, port = port })
            };
            var e = new Execute() {
                command = Execute.MyExecuteCommand
            };

            var s = new Send() {
                ip = ip,
                port = port,
                message = CommandHandler.CommandToString(e)
            };
            s.Inject(this);
            //s.ExecuteCommand();
        }

        public void Stop(AgentContact source)
        {
            Console.WriteLine("Agent is executing stop");
            new Sender(this, Config.LoggerContact, new Store(this) { value = string.Format("END {0}:{1} {2} by {3}:{4} {5}", IP, Port, TAG, source.ip, source.port, source.tag) }).Send(false);
            Console.WriteLine("Agent is stopped 1");
            Environment.Exit(0);
            Receiver.Stop();
        }

        public void AddCommand(Command command)
        {
            Commands[command.GetType().Name.ToLower()] = command;
            Console.WriteLine("Adding command {0}", command.GetType().Name);
        }

        public void AddContact(AgentContact contact)
        {
            AgentContactBook.Add(contact);
        }

        public void RemoveContact(AgentContact contact)
        {
            AgentContactBook.Remove(contact);
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

        public void SendFailed(string message, string ip, int port)
        {
            Debug.Log(string.Format("Message send failed {1}:{2} {0}", message, ip, port), Logger.Level.Warning);
            RemoveContact(new AgentContact() { ip = ip, port = port });
        }

        private void Receiver_OnReceive(object sender, ReceiveEventArgs e)
        {
            Command c = CommandHandler.GetCommand(CommandsArray, e.message);
            if(c == null) {
                Debug.Log("Unknown command " + e.message, Logger.Level.Error);
            }
            c.Inject(this);

            if(c is Ack) {
                bool received = SendOnAckReceived(c as Ack);
                OnAckReceived?.Invoke(this, new CommandEventArgs() { command = c });
            }
            else {
                Ack ack = new Ack(this) { message = e.message };
                string message = CommandHandler.CommandToString(ack);

                new Sender(null, c.sourceIp, c.sourcePort, message).Send(false); // send ack
                
                //if(!e.sourceIP.Equals(c.sourceIp) || e.sourcePort != c.sourcePort) {
                //    Debug.Log(string.Format("SourceIP not equals {0}:{1} != {2}:{3}", e.sourceIP, e.sourcePort, c.sourceIp, c.sourcePort), Logger.Level.Error);
                //}

                Debug.Log(string.Format("Command received {0}:{1} | {2}:{3} [{4}] - {5}: {6}", e.sourceIP, e.sourcePort, c.sourceIp, c.sourcePort, c.tag, c.type, e.message), Logger.Level.Command);
                c.ExecuteCommand(); // execute command

                OnCommandReceived?.Invoke(this, new CommandEventArgs() { command = c }); // invoke event

                AddContact(c.Source);
            }
        }

        private void LoadConfig(string config_path)
        {
            Config = ConfigData.Deserialize(config_path);

            Port = (Config.Port != 0) ? Config.Port : new Random().Next(Config.PortFrom, Config.PortTo);
            TAG = Config.Tag;

            for(int i = 0; i < Config.Contacts.Count; i++) {
                AgentContact c = Config.Contacts[i];
                AgentContactBook.Add(c);
            }
        }
    }
}
