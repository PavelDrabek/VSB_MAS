using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Agent.Communication
{
    public class Sender
    {
        public Receiver Receiver { get; private set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public string Message { get; private set; }

        public EventHandler<EventArgs> OnTimeOut;

        public Sender(Receiver receiver, string ip, int port, string message)
        {
            IP = ip;
            Port = port;
            Message = message;
            Receiver = receiver;
        }

        public void Send()
        {
            try {
                var remoteEP = new IPEndPoint(IPAddress.Parse(IP), Port);
                var dataToSend = Encoding.UTF8.GetBytes(Message);

                UdpClient udpSender = new UdpClient();
                udpSender.SendAsync(dataToSend, dataToSend.Length, remoteEP);

                Receiver.OnReceive += Receiver_OnReceive;
            } catch(Exception ex) {
                Console.WriteLine("Sender Exception: " + ex.Message);
            }
        }

        private void Receiver_OnReceive(object sender, ReceiveEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
