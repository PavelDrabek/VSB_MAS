using Agent.Commands;
using Agent.Communication;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Agent
{
    public class Agent
    {
        public string IP { get; private set; }
        public int Port { get; private set; }

        private Receiver receiver;

        public Dictionary<string, Command> Commands { get; private set; }

        public Agent(int port)
        {
            Port = port;
            IP = GetLocalIPAddress();

            Commands = new Dictionary<string, Command>();

            receiver = new Receiver(port);
            receiver.OnReceive += Receiver_OnReceive;

        }

        public void AddCommand(Command command)
        {
            Commands[command.GetType().Name.ToLower()] = command;
            Console.WriteLine("Adding command {0}", command.GetType().Name);
        }

        private void Receiver_OnReceive(object sender, ReceiveEventArgs e)
        {
            Console.WriteLine("{0} >> {1}", e.source, e.message);
            CommandHandler.HandleUnparsedCommand(this, e.message);
        }

        public void Start()
        {
            receiver.Start();
            Console.WriteLine("Agent is started on {0}:{1}", IP, Port);
        }

        public void Stop()
        {
            receiver.Stop();
            Console.WriteLine("Agent is stopped");
        }

        public bool HandleCommand(string command, params string[] args)
        {
            if(Commands.ContainsKey(command)) {
                return CommandHandler.HandleCommand(Commands[command], args);
            }

            Console.WriteLine("Unknown message");
            return false;
        }

        public Command GetCommand(string command)
        {
            command = command.ToLower();
            if(Commands.ContainsKey(command)) {
                return Commands[command];
            }
            return null;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach(var ip in host.AddressList) {
                if(ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}
