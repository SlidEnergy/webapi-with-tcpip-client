using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpIpServer
{
    public class Server
    {
        private TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 10000);
        private List<TcpClient> clients = new List<TcpClient>();
        private BackgroundWorker worker = new BackgroundWorker();
        private readonly CancellationTokenSource stoppingCts = new CancellationTokenSource();

        public event Action<string> DebugMessage;
        public event Action<string> RecieveMessage;

        public Server()
        {
            worker.DoWork += Worker_DoWork;
            
        }

        private async void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            await RunLoop(stoppingCts.Token);
        }

        public void Run()
        {
            listener.Start();
            worker.RunWorkerAsync();
        }

        private async Task RunLoop(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                clients.Add(client);

                var session = new ClientSession(client);
                session.RecieveMessage += (msg) => RecieveMessage?.Invoke(msg);

                // создаем новый поток для обслуживания нового клиента
                Thread clientThread = new Thread(new ThreadStart(session.Process));
                clientThread.Start();

                DebugMessage?.Invoke("Connected");
            }
        }

        public async Task SendBroadcast(string message, CancellationToken cancellationToken = default)
        {
            var data = Encoding.UTF8.GetBytes(message);

            foreach (var client in clients)
            {
                var networkStream = client.GetStream();

                await networkStream.WriteAsync(data, 0, data.Length, cancellationToken);
            }

            DebugMessage?.Invoke($"Send: {message}");
        }
    }
}
