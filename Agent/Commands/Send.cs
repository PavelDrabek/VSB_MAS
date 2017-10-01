using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class Send : Command
    {
        public string Receiver { get; set; }
        public string Content { get; set; }

        public void Call()
        {
            Console.WriteLine("Working: ");
        }

        public void Call(string receiver, string message)
        {
            string[] receiverSplit = receiver.Split(':');

            try {
                string ip = receiverSplit[0];
                int port = int.Parse(receiverSplit[1]);

                SendMessage(ip, port, message);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public static void SendMessage(string ip, int port, string message)
        {
            try {
                var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
                var dataToSend = Encoding.UTF8.GetBytes(message);

                UdpClient udpSender = new UdpClient();
                udpSender.SendAsync(dataToSend, dataToSend.Length, remoteEP);
            } catch(Exception ex) {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}
