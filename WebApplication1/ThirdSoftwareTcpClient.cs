using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class ThirdSoftwareTcpClient : IThirdSoftwareTcpClient, IDisposable
    {
        private TcpClient _tcpClient;
        private readonly IOptions<ThirdSoftwareConfig> _config;

        public ThirdSoftwareTcpClient(IOptions<ThirdSoftwareConfig> config)
        {
            _tcpClient = new TcpClient();
            _config = config;
        }

        public async Task ConnectAsync()
        {
            var config = _config.Value;

            if (string.IsNullOrEmpty(config.Server) || config.Port == 0)
                throw new Exception("Third software server or port not defined");

            await _tcpClient.ConnectAsync(config.Server, config.Port);
        }

        public async Task SendAsync(byte[] data)
        {
            var networkStream = _tcpClient.GetStream();
            await networkStream.WriteAsync(data, 0, data.Length);
        }

        public async Task<byte[]> ReceiveAsync()
        {
            using var memoryStream = new MemoryStream();

            byte[] data = new byte[256];
            var networkStream = _tcpClient.GetStream();

            do
            {
                int bytes = await networkStream.ReadAsync(data, 0, data.Length);
                await memoryStream.WriteAsync(data, 0, bytes);
            }
            while (networkStream.DataAvailable);

            return memoryStream.ToArray();
        }

        public void Dispose()
        {
            _tcpClient?.Dispose();
        }
    }
}
