using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpIpServer
{
    public class ClientSession
    {
        public TcpClient client;
        public event Action<string> RecieveMessage;
        public string Id = Guid.NewGuid().ToString();

        public ClientSession(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[256]; // буфер для получаемых данных
                while (true)
                {
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();

                    RecieveMessage?.Invoke($"Session {Id} Received: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}
