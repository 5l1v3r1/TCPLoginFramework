using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }

        public string ip = "192.168.0.15";
        public string port = "1337";
        public string message;

        SimpleTcpClient client;

        private void Client_Load(object sender, EventArgs e)
        {
            client = new SimpleTcpClient();
            client.StringEncoder = Encoding.UTF8;
            client.DataReceived += clientDataRecieved;

            try
            {
                client.Connect(ip, Convert.ToInt32(port));
            }
            catch
            {
                this.Hide();
                MessageBox.Show("Could not connect to the server!" + Environment.NewLine + "Please ensure your firewall is disabled.", "Client", MessageBoxButtons.OK);
                Application.Exit();
            }
        }

        private void clientDataRecieved(object sender, SimpleTCP.Message e)
        {
            message = e.MessageString.Remove(e.MessageString.Length - 1);

            if (message == "Valid")
            {
                MessageBox.Show("Login information valid");
            }

            if (message == "Invalid")
            {
                MessageBox.Show("Login information invalid!", "oof", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            client.WriteLineAndGetReply("Login:" + textBox1.Text + "╥" + textBox2.Text, TimeSpan.FromMilliseconds(100));
        }
    }
}
