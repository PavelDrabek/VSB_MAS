using Agent.Utilities;
using System;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Agent.Communication
{
    public class ReceiveEventArgs : EventArgs
    {
        public string sourceIP;
        public int sourcePort;
        public string message;
    }

    public class Receiver
    {
        private Thread thread;

        private UdpClient client;

        public EventHandler<ReceiveEventArgs> OnReceive;

        public Receiver(int port)
        {
            client = new UdpClient(port, AddressFamily.InterNetwork);

            thread = new Thread(Work);
        }

        public void Start()
        {
            thread.Start();
        }

        public void Stop()
        {
            thread.Abort();
        }

        static int x;
        private void Work()
        {
            string message = "";
            string ip = "";
            int port = -1;

            while(true) {

                try {
                    var dataReceived = client.ReceiveAsync();
                    message = GetString(dataReceived.Result.Buffer);
                    ip = dataReceived.Result.RemoteEndPoint.Address.ToString();
                    port = dataReceived.Result.RemoteEndPoint.Port;
                } catch(Exception ex) {
                    //Console.WriteLine("Receiver Exception: " + ex.Message);
                    Debug.Log("Receiver Exception: " + ex.Message, Logger.Level.Error);
                }

                //Console.WriteLine("Received {0}", x++);
                OnReceive?.Invoke(this, new ReceiveEventArgs() {
                    message = message,
                    sourceIP = ip,
                    sourcePort = port
                });
            }
        }

        private string GetString(byte[] bytes)
        {
            string str = Encoding.UTF8.GetString(bytes);
            String unescapedString = Regex.Unescape(str);
            //return unescapedString;
            return str;
        }
    }
}
