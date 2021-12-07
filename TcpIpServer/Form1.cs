using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TcpIpServer
{
    public partial class Form1 : Form
    {
        private Server server = new Server();

        public Form1()
        {
            InitializeComponent();

            server.DebugMessage += Server_DebugMessage;
            server.RecieveMessage += Server_RecieveMessage;
        }

        private void Server_RecieveMessage(string obj)
        {
            if (this.InvokeRequired)
            {
                Action d = delegate { Server_RecieveMessage(obj); };
                this.Invoke(d);
            }
            else
                textBox1.Text += "recieve " + obj + Environment.NewLine;
        }

        private void Server_DebugMessage(string obj)
        {
            if (this.InvokeRequired)
            {
                Action d = delegate { Server_DebugMessage(obj); };
                this.Invoke(d);
            }
            else
                textBox1.Text += "debug " + obj + Environment.NewLine;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            server.Run();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            textBox2.Focus();

            await server.SendBroadcast(textBox2.Text);
        }
    }
}
