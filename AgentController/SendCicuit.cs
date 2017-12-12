using Agent;
using Agent.Commands;
using Agent.Communication;
using Agent.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgentController
{
    public partial class SendCicuit : Form
    {
        int Port = 12345;
        Thread t;

        public SendCicuit()
        {
            InitializeComponent();
            t = new Thread(Receiver);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try {
                label3.Text = string.Format("Status: Parsing addresses");

                var addresses = tbAddresses.Text.Split('\n');
                List<AgentContact> contacts = new List<AgentContact>();
                for(int i = 0; i < addresses.Length; i++) {
                    var ip = addresses[i].Split(':')[0];
                    var port = int.Parse(addresses[i].Split(':')[1]);
                    contacts.Add(new AgentContact() { ip = ip, port = port });
                }

                SendAddresses(contacts);
            } catch(Exception ex) {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private void SendAddresses(List<AgentContact> contacts)
        {
            if(contacts.Count < 2) {
                MessageBox.Show("Two or more addresses are needed");
                return;
            }

            label3.Text = string.Format("Status: Building command");
            Command command = new Store() { value = "start message" };

            for(int i = contacts.Count - 1; i >= 0; i--) {
                var c = contacts[i];

                var nCom = new Send() { ip = c.ip, port = c.port, message = CommandHandler.CommandToString(command) };
                command = nCom;
            }

            command.sourceIp = IPHelper.GetLocalIPAddress();
            command.sourcePort = Port;
            command.tag = "Initializer";

            string message = CommandHandler.CommandToString(command);
            textBox1.Text = message;

            SendCommand(contacts[0], message);
        }

        private void SendCommand(AgentContact receiver, string message)
        {
            var client = new UdpClient();

            try {
                var remoteEP = new IPEndPoint(IPAddress.Parse(receiver.ip), receiver.port);
                var dataToSend = Encoding.UTF8.GetBytes(message);
                
                client.SendAsync(dataToSend, dataToSend.Length, remoteEP);
                label3.Text = string.Format("Status: Send to {0}:{1}", receiver.ip, receiver.port);

            } catch(Exception ex) {
                MessageBox.Show("Exception: " + ex.Message);
            }

            t.Start();
        }

        private string GetString(byte[] bytes)
        {
            string str = Encoding.UTF8.GetString(bytes);
            String unescapedString = Regex.Unescape(str);
            //return unescapedString;
            return str;
        }

        private void Receiver()
        {
            var client = new UdpClient(Port, AddressFamily.InterNetwork);
            //label3.Text = string.Format("Status: Receiving...");
            try {
                var dataReceived = client.ReceiveAsync();
                var message = GetString(dataReceived.Result.Buffer);
                var s_ip = dataReceived.Result.RemoteEndPoint.Address.ToString();
                var s_port = dataReceived.Result.RemoteEndPoint.Port;

                var ack = CommandHandler.GetCommand(new Command[] { new Ack() }, message);
                if(ack is Ack) {
                    MessageBox.Show(string.Format("Received ACK from {0}:{1}", s_ip, s_port));
                    //label3.Text = string.Format("Status: Received ACK from {0}:{1}", s_ip, s_port);
                }

            } catch(Exception ex) {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }
    }
}
