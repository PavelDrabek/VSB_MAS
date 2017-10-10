using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Agent.Communication
{
    public class ReceiveEventArgs : EventArgs
    {
        public string fromIP;
        public int fromPort;
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
                    Console.WriteLine("Receiver Exception: " + ex.Message);
                }

                OnReceive?.Invoke(this, new ReceiveEventArgs() {
                    message = message,
                    fromIP = ip,
                    fromPort = port
                });
            }
        }

        private string GetString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
