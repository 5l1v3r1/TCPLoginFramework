using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using SimpleTCP;

namespace TCP_Login_Handler
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }

        int parsedValue;
        bool serverStarted = false;
        SimpleTcpServer server;

        private void Form1_Load(object sender, EventArgs e)
        {
            String strHostName = Dns.GetHostName();

            IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                comboBox1.Items.Add(ipaddress);
            }
        }

        private void LogInformation(string LogMessage)
        {
            logBox.Items.Add("[" + DateTime.Now.ToString("HH:mm:ss tt") + "] " + LogMessage);

            int visibleItems = logBox.ClientSize.Height / logBox.ItemHeight;
            logBox.TopIndex = Math.Max(logBox.Items.Count - visibleItems + 1, 0);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Error: Please select an IP from the list", "Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (!int.TryParse(textBox1.Text, out parsedValue))
            {
                MessageBox.Show("Error: Only numbers are allowed in the server port entry", "Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (parsedValue >= 65535 || parsedValue <= 1023)
            {
                MessageBox.Show("Error: Port is not in range!" + Environment.NewLine + "Allowed range: 1024-65535", "Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!serverStarted)
            {
                serverStarted = true;

                server = new SimpleTcpServer();
                server.Delimiter = 0x13;
                server.StringEncoder = Encoding.UTF8;
                server.DataReceived += serverDataRecieved;

                System.Net.IPAddress ip = System.Net.IPAddress.Parse(comboBox1.Text);
                server.Start(ip, Convert.ToInt32(textBox1.Text));

                button1.Text = "Stop Server";
                LogInformation("Server started on port " + textBox1.Text);
            }
            else
            {
                serverStarted = false;

                server.Stop();

                button1.Text = "Start Server";
                LogInformation("Server stopped");
            }
        }

        string message;

        private void serverDataRecieved(object sender, SimpleTCP.Message e)
        {
            message = e.MessageString.Remove(e.MessageString.Length - 1);

            if (message.Contains("Login"))
            {
                string[] splitMessage = message.Split(':');
                string[] splitLoginInfo = splitMessage[1].Split('╥');

                var web = new WebClient();
                var result = web.DownloadString(textBox2.Text + "?username=" + splitLoginInfo[0] + "&password=" + splitLoginInfo[1]);

                if (result != null && result.Contains("0"))
                {
                    e.ReplyLine("Invalid");
                }

                if (result != null && result.Contains("1"))
                {
                    e.ReplyLine("Valid");
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Uri uriResult;
            bool confirmUrl = Uri.TryCreate(textBox2.Text, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (textBox2.Text == "")
            {
                MessageBox.Show("Login script URL is empty");
            }
            else if (confirmUrl)
            {
                if (textBox2.Text.Contains("?") || textBox1.Text.Contains("&"))
                {
                    MessageBox.Show("Login script field contains too much data. ONLY include http(s)://yourdomain.com/login.php or wherever your login script lands!");
                }
                else if (!textBox2.Text.Contains("/login.php"))
                {
                    MessageBox.Show("Login script field does not contain enough data. Please include http(s)://yourdomain.com/login.php or wherever your login script lands!");
                }
                else
                {
                    var web = new WebClient();
                    var result = web.DownloadString(textBox2.Text + "?username=LoaderTesting&password=LoaderTesting");

                    if (result != null && result.Contains("TestTrue"))
                    {
                        LogInformation("Web script confirmed working!");
                        MessageBox.Show("Login script works fine!", "Server");
                    }
                    else if (result != null && result.Contains("Connection failed: Access denied"))
                    {
                        MessageBox.Show("The SQL login information on your script is incorrect! You need to fix this for the logins to work properly!");
                    }
                    else
                    {
                        MessageBox.Show("Unknown PHP error!" + Environment.NewLine + "PHP Code: " + result);
                    }
                }
            }
            else
            {
                MessageBox.Show("URL is not in the proper format!");
            }
        }
    }
}
