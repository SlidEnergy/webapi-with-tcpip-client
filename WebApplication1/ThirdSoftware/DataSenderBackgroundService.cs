using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiWithTcpIpClient
{
    public class DataSenderBackgroundService : TimedBackgroundService
    {
        public IServiceProvider Services { get; }

        public DataSenderBackgroundService(ILogger<DataSenderBackgroundService> logger, IServiceProvider services) : base(logger)
        {
            Services = services;
            DueTime = 0;
            Period = 30;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = Services.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<IThirdSoftwareClient>();
            var someRepository = scope.ServiceProvider.GetRequiredService<SomeRepository>();

            var data = someRepository.GetData();

            await client.EnsureConnectedAsync(cancellationToken);

            await client.SendAsync(data.Serialize(), cancellationToken);
        }
    }
}
