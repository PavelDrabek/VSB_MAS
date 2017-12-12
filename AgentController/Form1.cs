using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgentController
{
    public partial class Form1 : Form
    {

        UdpClient udpSender;

        public Form1()
        {
            InitializeComponent();
            udpSender = new UdpClient();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            CommandForm commandForm = new CommandForm();
            commandForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try {
                string ip = txtAdress.Text.Split(':')[0];
                int port = int.Parse(txtAdress.Text.Split(':')[1]);
                string message = txtMessage.Text;
                try {
                    var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
                    var dataToSend = Encoding.UTF8.GetBytes(message);

                    udpSender.SendAsync(dataToSend, dataToSend.Length, remoteEP);
                } catch(Exception ex) {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            } catch(Exception ee) {
                MessageBox.Show(ee.Message);
            }
        }

        private void btnCircuit_Click(object sender, EventArgs e)
        {
            SendCicuit commandForm = new SendCicuit();
            commandForm.Show();
        }
    }
}
