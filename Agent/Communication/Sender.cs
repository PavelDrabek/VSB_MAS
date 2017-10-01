using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Communication
{
    public class Sender
    {
        public void Send(string ip, int port, string message)
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
