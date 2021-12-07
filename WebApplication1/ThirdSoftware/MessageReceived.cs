using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiWithTcpIpClient
{
    public class MessageReceived : IDisposable
    {
        private readonly IThirdSoftwareClient _client;
        private readonly ILogger<MessageReceived> _logger;

        public event Action<string> RecieveMessage;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public MessageReceived(IThirdSoftwareClient client, ILogger<MessageReceived> logger)
        {
            this._client = client;
            _logger = logger;
        }

        //public async Task<string> SendData(byte[] data, CancellationToken cancellationToken)
        //{
        //    await _client.ConnectAsync(cancellationToken);

        //    await _client.SendAsync(data, cancellationToken);

        //    var response = await _client.ReceiveAsync(cancellationToken);
        //    var responseMessage = System.Text.Encoding.UTF8.GetString(response);

        //    return responseMessage;
        //}

        public async Task Run(CancellationToken cancellationToken)
        {
            await _client.EnsureConnectedAsync(cancellationToken);

            await RunLoop(cancellationToken);
        }

        private async Task RunLoop(CancellationToken cancellationToken)
        { 
            while(!cancellationToken.IsCancellationRequested && !_stoppingCts.IsCancellationRequested)
            {
                var response = await _client.ReceiveAsync(cancellationToken);
                var responseMessage = System.Text.Encoding.UTF8.GetString(response);

                _logger.LogInformation("Receive message: " + responseMessage);

                RecieveMessage?.Invoke(responseMessage);
            }
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
            _client?.Dispose();
        }
    }
}
