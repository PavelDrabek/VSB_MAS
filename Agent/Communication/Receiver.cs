using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agent.Communication
{
    public class ReceiveEventArgs : EventArgs
    {
        public string source;
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
            while(true) {
                try {
                    var dataReceived = client.ReceiveAsync();
                    var message = Encoding.UTF8.GetString(dataReceived.Result.Buffer);
                    OnReceive?.Invoke(this, new ReceiveEventArgs() { message = message, source = dataReceived.Result.RemoteEndPoint.Address.ToString() });
                } catch(Exception ex) {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }
        }
    }
}
