using TaskManagement.Services;

namespace TaskManagement.Services
{
    public class TaskStatusUpdateService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TaskStatusUpdateService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Run daily

        public TaskStatusUpdateService(
            IServiceProvider services,
            ILogger<TaskStatusUpdateService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Task Status Update Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting task status check at: {time}", DateTimeOffset.Now);

                    using (var scope = _services.CreateScope())
                    {
                        var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
                        await taskService.UpdateOverdueTasksAsync();
                    }

                    _logger.LogInformation("Completed task status check at: {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating task statuses");
                }

                // Wait for the next check interval
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task Status Update Service is starting up");
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task Status Update Service is stopping");
            await base.StopAsync(cancellationToken);
        }
    }
}