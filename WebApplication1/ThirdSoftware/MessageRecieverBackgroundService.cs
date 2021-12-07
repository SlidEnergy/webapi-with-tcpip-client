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
		private readonly MessageReciever _messageManager;

		public MessageRecieverBackgroundService(MessageReciever messageManager)
		{
			_messageManager = messageManager;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await _messageManager.Run(stoppingToken);
		}
	}
}
