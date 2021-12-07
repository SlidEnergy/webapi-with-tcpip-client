using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiWithTcpIpClient
{
    public class ThirdSoftwareClient : IThirdSoftwareClient, IDisposable
    {
        private TcpClient _tcpClient;
        private readonly IOptions<ThirdSoftwareConfig> _config;

        public ThirdSoftwareClient(IOptions<ThirdSoftwareConfig> config)
        {
            _tcpClient = new TcpClient();
            _config = config;
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            var config = _config.Value;

            if (string.IsNullOrEmpty(config.Server) || config.Port == 0)
                throw new Exception("Third software server or port not defined");

            await _tcpClient.ConnectAsync(config.Server, config.Port, cancellationToken);
        }

        public Task EnsureConnectedAsync(CancellationToken cancellationToken = default)
        {
            if (_tcpClient.Connected)
                return Task.CompletedTask;

            return ConnectAsync(cancellationToken);
        }

        public async Task SendAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var networkStream = _tcpClient.GetStream();
            await networkStream.WriteAsync(data, 0, data.Length, cancellationToken);
        }

        public async Task<byte[]> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            using var memoryStream = new MemoryStream();

            byte[] data = new byte[256];
            var networkStream = _tcpClient.GetStream();

            do
            {
                int bytes = await networkStream.ReadAsync(data, 0, data.Length, cancellationToken);
                await memoryStream.WriteAsync(data, 0, bytes, cancellationToken);
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
