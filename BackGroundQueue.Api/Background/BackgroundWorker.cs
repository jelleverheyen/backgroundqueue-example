using System;
using System.Threading;
using System.Threading.Tasks;
using BackgroundQueue.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackGroundQueue.Api.Background
{
    public class BackgroundWorker : BackgroundService
    {
        private readonly IBackgroundQueue<Book> _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BackgroundWorker> _logger;

        public BackgroundWorker(IBackgroundQueue<Book> queue, IServiceScopeFactory scopeFactory, 
            ILogger<BackgroundWorker> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Type} is now running in the background.", nameof(BackgroundWorker));

            await BackgroundProcessing(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical(
                "The {Type} is stopping due to a host shutdown, queued items might not be processed anymore.",
                nameof(BackgroundWorker)
            );

            return base.StopAsync(cancellationToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(500, stoppingToken);
                    var book = _queue.Dequeue();

                    if (book == null) continue;

                    _logger.LogInformation("Book found! Starting to process ..");

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var publisher = scope.ServiceProvider.GetRequiredService<IBookPublisher>();

                        await publisher.Publish(book, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical("An error occurred when publishing a book. Exception: {@Exception}", ex);
                }
            }
        }
    }
}