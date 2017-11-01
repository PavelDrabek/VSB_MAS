using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agent.Communication
{
    public class Sender
    {
        public Agent Agent { get; private set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public string Message { get; private set; }

        public bool Acknowledged { get; private set; }
        public EventHandler<EventArgs> OnTimeOut;

        public int millisecondsTimeout = 5000;
        private Thread ackThread;

        public Sender(Agent receiver, string ip, int port, string message)
        {
            IP = ip;
            Port = port;
            Message = message;
            Agent = receiver;

            Acknowledged = false;
            ackThread = new Thread(SetACK);
        }

        public void Send(bool needsAck = true)
        {
            try {
                var remoteEP = new IPEndPoint(IPAddress.Parse(IP), Port);
                var dataToSend = Encoding.UTF8.GetBytes(Message);

                UdpClient udpSender = new UdpClient();
                udpSender.SendAsync(dataToSend, dataToSend.Length, remoteEP);

                if(needsAck && Agent != null) {
                    ackThread.Start();
                }
            } catch(Exception ex) {
                Console.WriteLine("Sender Exception: " + ex.Message);
            }
        }

        public static void SendACK(string ip, int port, string message)
        {
            try {
                var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
                var dataToSend = Encoding.UTF8.GetBytes(message);

                UdpClient udpSender = new UdpClient();
                udpSender.SendAsync(dataToSend, dataToSend.Length, remoteEP);
            } catch(Exception ex) {
                Console.WriteLine("Sender Exception: " + ex.Message);
            }
        }

        private void SetACK()
        {   
            //Console.WriteLine("SetACK {0}", Message);
            Agent.OnAckReceived += Receiver_OnAckReceived;
            for(int i = 0; i < 5; i++) {
                Thread.Sleep(millisecondsTimeout);
                Send(false);
                //Console.WriteLine("Timeout {0}", Message);
            }
            Agent.OnAckReceived -= Receiver_OnAckReceived;
            Console.WriteLine("Message send failed: {0}", Message);
        }

        private void Receiver_OnAckReceived(object sender, ReceiveEventArgs e)
        {
            if(e.message.ToLower() != Message.ToLower()) {
                //Console.WriteLine("Acknowledged not for me {0}", Message);
                return;
            }
            Acknowledged = true;
            Agent.OnAckReceived -= Receiver_OnAckReceived;
            ackThread.Abort();
            //Console.WriteLine("Acknowledged {0}", Message);
        }
    }
}
