using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiWithTcpIpClient
{
    public class MessageRecieverBackgroundService : BackgroundService
	{
		private readonly MessageReceived _messageReceived;
        private readonly IThirdSoftwareClient client;

        public MessageRecieverBackgroundService(MessageReceived messageReceived, IThirdSoftwareClient client)
		{
			_messageReceived = messageReceived;
            _messageReceived.RecieveMessage += _messageReceived_RecieveMessage;
            this.client = client;
        }

        private async void _messageReceived_RecieveMessage(string obj)
        {
            var data = System.Text.Encoding.UTF8.GetBytes(obj.ToUpper());

            await client.SendAsync(data);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await _messageReceived.Run(stoppingToken);
		}
	}
}
