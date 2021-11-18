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
    public class TaskRunnerBackgroundService : IHostedService
    {
        private int executionCount = 0;
        private readonly ILogger<TaskRunnerBackgroundService> _logger;
        private Timer _timer = null!;
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        public IServiceProvider Services { get; }

        public TaskRunnerBackgroundService(ILogger<TaskRunnerBackgroundService> logger, IServiceProvider services)
        {
            _logger = logger;
            Services = services;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(30));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            try
            {
                var count = Interlocked.Increment(ref executionCount);

                _logger.LogInformation(
                    "Timed Hosted Service is working. Count: {Count}", count);

                _timer?.Change(Timeout.Infinite, 0);

                _executingTask = DoWorkAsync(_stoppingCts.Token);
            }
            catch(Exception ex)
            {
                // Handle exception
                _logger.LogInformation("Thrown exception " + ex.Message);
            }
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            using (var scope = Services.CreateScope())
            {
                var thirdSoftwareService = scope.ServiceProvider.GetRequiredService<IThirdSoftwareService>();
                var someRepository = scope.ServiceProvider.GetRequiredService<SomeRepository>();

                var data = someRepository.GetData();

                var response = await thirdSoftwareService.SendData(data.Serialize());
            }

            _timer.Change(TimeSpan.FromSeconds(30), TimeSpan.FromMilliseconds(-1));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
            _timer?.Dispose();
        }
    }
}
