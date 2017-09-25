using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8888;
            string ip = "127.0.0.1";

            UdpClient udpSender = new UdpClient();

            Console.WriteLine("Client Started");

            while(true) {
                string message = Console.ReadLine();
                try {
                    //udpSender = new UdpClient();

                    var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
                    var msgToPrint = message;
                    var msgToSend = msgToPrint;
                    var dataToSend = Encoding.UTF8.GetBytes(msgToSend);

                    udpSender.SendAsync(dataToSend, dataToSend.Length, remoteEP);
                } catch(Exception ex) {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }

            

        }
    }
}
