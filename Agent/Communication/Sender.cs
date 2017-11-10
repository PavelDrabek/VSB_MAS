using Agent.Commands;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agent.Communication
{
    public class Sender : IAckReceiver
    {
        public Agent Agent { get; private set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public string Message { get; private set; }

        public bool Acknowledged { get; private set; }
        public EventHandler<EventArgs> OnTimeOut;

        private static UdpClient client;
        private static UdpClient Client { get { if(client == null) { client = new UdpClient(); } return client; } }

        private int timeoutCount;
        private int secondCount;

        public Sender(Agent receiver, string ip, int port, Command command) : this(receiver, ip, port, CommandHandler.CommandToString(command))
        {
        }

        public Sender(Agent receiver, string ip, int port, string message)
        {
            IP = ip;
            Port = port;
            Message = message;
            Agent = receiver;

            Acknowledged = false;

            timeoutCount = 0;
            secondCount = 0;
        }

        static int x = 0, y = 0;
        public void Send(bool needsAck = true)
        {
            try {
                var remoteEP = new IPEndPoint(IPAddress.Parse(IP), Port);
                var dataToSend = Encoding.UTF8.GetBytes(Message);

                if(needsAck && Agent != null) {
                    StartAck();
                }
                //UdpClient udpSender = new UdpClient();
                Client.SendAsync(dataToSend, dataToSend.Length, remoteEP);
                //Console.WriteLine("Send {0} {1}", x++, Message);
                //Console.WriteLine("Send {0}", x++);

            } catch(Exception ex) {
                Console.WriteLine("Sender Exception: " + ex.Message);
            }
        }

        //public static void SendACK(string ip, int port, string message)
        //{
        //    try {
        //        var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
        //        var dataToSend = Encoding.UTF8.GetBytes(message);

        //        UdpClient udpSender = new UdpClient();
        //        udpSender.SendAsync(dataToSend, dataToSend.Length, remoteEP);
        //    } catch(Exception ex) {
        //        Console.WriteLine("Sender Exception: " + ex.Message);
        //    }
        //}

        private void StartAck()
        {   
            //Console.WriteLine("SetACK {0}", Message);
            //Agent.OnAckReceived += Receiver_OnAckReceived;
            Agent.ackReceivers.Add(this);
            Agent.OnSecondTick += OnTick;
            //for(int i = 0; i < 5; i++) {
            //    Thread.Sleep(millisecondsTimeout);
            //    Send(false);
            //    //Console.WriteLine("Timeout {0}", Message);
            //}
            //Agent.OnAckReceived -= Receiver_OnAckReceived;
            //Agent.OnSecondTick -= OnTick;
            //Console.WriteLine("Message send failed: {0}", Message);
        }

        private void StopACK()
        {
            //ackThread.Abort();
            //Agent.OnAckReceived -= Receiver_OnAckReceived;
            Agent.OnSecondTick -= OnTick;
        }

        private void Receiver_OnAckReceived(object sender, ReceiveEventArgs e)
        {
            if(e.message.ToLower() != Message.ToLower()) {
                //Console.WriteLine("Acknowledged not for me {0}", Message);
                return;
            }

            //Console.WriteLine("Acknowledged {0}", y++);

            Acknowledged = true;
            StopACK();
            //Console.WriteLine("Acknowledged {0}", Message);
            //Console.WriteLine("Acknowledged");
        }

        private void OnTick(object sender, EventArgs e)
        {
            secondCount++;
            if(secondCount >= 5) {
                secondCount = 0;
                timeoutCount++;
                Console.WriteLine("Message timeout");

                if(timeoutCount > 50) {
                    StopACK();
                    Console.WriteLine("Message send failed: {0}", Message);
                } else {
                    Send(false);
                }
            }
            //Console.WriteLine("Acknowledged {0}", Message);
        }

        public bool ReceiveACK(string sourceIP, int sourcePort, string message)
        {
            if(message.ToLower() != Message.ToLower()) {
                //Console.WriteLine("Acknowledged not for me {0}", Message);
                return false;
            }

            //Console.WriteLine("Acknowledged {0}", y++);

            Acknowledged = true;
            StopACK();
            return true;
        }
    }
}
