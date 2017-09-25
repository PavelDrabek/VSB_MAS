using Agent.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Agent
{
    public class Agent
    {
        public int Port { get; private set; }
        public UdpClient UdpReceiver { get; private set; }
        public bool shouldStop { get; private set; }

        public Dictionary<string, Command> Commands { get; private set; }

        public Agent(int port)
        {
            Port = port;

            Commands = new Dictionary<string, Command>();
            UdpReceiver = new UdpClient(port, AddressFamily.InterNetwork);
        }

        public void AddCommand(Command command)
        {
            Commands[command.GetType().Name.ToLower()] = command;
            Console.WriteLine("Adding command {0}", command.GetType().Name);

        }

        public void Execute()
        {
            shouldStop = false;
            Console.WriteLine("Agent running...");

            while(!shouldStop) {
                try {
                    var dataReceived = UdpReceiver.ReceiveAsync();
                    var message = Encoding.UTF8.GetString(dataReceived.Result.Buffer).Replace("\"", "");

                    Console.WriteLine("{0} >> {1}", dataReceived.Result.RemoteEndPoint.Address, message);
                    if(!HandleUnparsedCommand(message)) {
                        Console.WriteLine("Unknown message");
                    }

                } catch(Exception ex) {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }

            Console.WriteLine("Agent stopped");
        }

        public void Stop()
        {
            shouldStop = true;
        }

        public bool HandleUnparsedCommand(string text)
        {
            string[] parts = text.Split(' ');

            if(parts.Length > 0) {
                if(parts.Length > 1) {
                    string[] args = new string[parts.Length - 1];

                    for(int i = 0; i < parts.Length - 1; i++)
                        args[i] = parts[i + 1];

                    return HandleCommand(parts[0].ToLower(), args);
                } else {
                    return HandleCommand(parts[0].ToLower());
                }
            }

            return false;
        }

        public bool HandleCommand(string command, params string[] args)
        {
            if(Commands.ContainsKey(command)) {
                return CommandHandler.HandleCommand(Commands[command], args);
            }

            return false;
        }

    }
}
