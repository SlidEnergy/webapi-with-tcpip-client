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
    /// <summary>
    /// Invoke ExecuteAsync at specified intervals.
    /// </summary>
    public abstract class TimedBackgroundService : IHostedService
    {
        private int executionCount = 0;
        private readonly ILogger<DataSenderBackgroundService> _logger;
        private Timer _timer = null!;
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        /// <summary>
        /// The time interval between invocations in miliseconds
        /// </summary>
        public int Period { get; set; } = 1000;

        /// <summary>
        /// The amount of time to delay before the callback is invoked.
        /// </summary>
        public int DueTime { get; set; } = 0;

        public TimedBackgroundService(ILogger<DataSenderBackgroundService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(Execute, null, TimeSpan.FromMilliseconds(DueTime), TimeSpan.FromMilliseconds(Period));

            return Task.CompletedTask;
        }

        private void Execute(object state)
        {
            try
            {
                var count = Interlocked.Increment(ref executionCount);

                _logger.LogInformation(
                    "Timed Hosted Service is working. Count: {Count}", count);

                _timer?.Change(Timeout.Infinite, 0);

                _executingTask = ExecuteAsync(_stoppingCts.Token);

                _executingTask.ContinueWith(task => _timer.Change(TimeSpan.FromSeconds(Period), TimeSpan.FromMilliseconds(Timeout.Infinite)));
            }
            catch(Exception ex)
            {
                // Handle exception
                _logger.LogInformation("Thrown exception " + ex.Message);
            }
        }

        public abstract Task ExecuteAsync(CancellationToken cancellationToken);

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
