using Agent.Commands;
using Agent.Utilities;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Agent.Communication
{
    public class Sender : IAckReceiver
    {
        public Agent Agent { get; private set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public Command Command { get; private set; }
        public string Message { get; private set; }

        public bool Acknowledged { get; private set; }
        public EventHandler<EventArgs> OnTimeOut;

        private static UdpClient client;
        private static UdpClient Client { get { if(client == null) { client = new UdpClient(); } return client; } }

        private int timeoutCount;
        private int secondCount;

        public Sender(Agent ackReceiver, string ip, int port, Command command) : this(ackReceiver, ip, port, CommandHandler.CommandToString(command))
        {
        }

        public Sender(Agent ackReceiver, AgentContact contact, Command command) : this(ackReceiver, contact.ip, contact.port, command)
        {

        }

        public Sender(Agent ackReceiver, string ip, int port, string message)
        {
            IP = ip;
            Port = port;
            Message = message;
            Agent = ackReceiver;

            Acknowledged = false;

            timeoutCount = 0;
            secondCount = 0;
        }

        public void Send(bool needsAck = true)
        {
            try {
                var remoteEP = new IPEndPoint(IPAddress.Parse(IP), Port);
                var dataToSend = Encoding.UTF8.GetBytes(Message);

                if(needsAck) {
                    Debug.Log(string.Format("Command send {0}:{1} - {2}", IP, Port, Message), Logger.Level.Command);
                }

                if(needsAck && Agent != null) {
                    StartAck();
                }
                Client.SendAsync(dataToSend, dataToSend.Length, remoteEP);

            } catch(Exception ex) {
                //Console.WriteLine("Sender Exception: " + ex.Message);
                Debug.Log("Sender Exception: " + ex.Message, Logger.Level.Error);
            }
        }

        private void StartAck()
        {   
            Agent.ackReceivers.Add(this);
            Agent.OnSecondTick += OnTick;
        }

        private void StopACK()
        {
            Agent.OnSecondTick -= OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if(Acknowledged) {
                return;
            }

            secondCount++;
            if(secondCount >= 5) {
                secondCount = 0;
                timeoutCount++;
                //Console.WriteLine("Message timeout {1}:{2} {0}", Message, IP, Port);

                if(timeoutCount > 5) {
                    StopACK();
                    Agent.SendFailed(Message, IP, Port);
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
