using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VSB_MAS
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8888;
            var udpReceiver = new UdpClient(port, AddressFamily.InterNetwork);

            int requestCount = 0;
            Console.WriteLine(" >> Agent Started");
            Console.WriteLine(" >> Accept connection from client");

            while((true)) {
                Console.WriteLine(" >> Waiting for message");

                try {
                    var dataReceived = udpReceiver.ReceiveAsync();
                    var msgReceived = Encoding.UTF8.GetString(dataReceived.Result.Buffer).Replace("\"", "");
                    Console.WriteLine("{0} >> {1}", dataReceived.Result.RemoteEndPoint.Address, msgReceived);
                } catch(Exception ex) {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }

            Console.WriteLine(" >> exit");
            Console.ReadLine();
        }
    }
}
