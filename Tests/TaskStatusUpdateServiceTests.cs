using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Services;
using Xunit;

namespace TaskManagement.Tests
{
    public class TaskStatusUpdateServiceTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldCallUpdateOverdueTasks()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            var mockLogger = new Mock<ILogger<TaskStatusUpdateService>>();

            var services = new ServiceCollection();
            services.AddScoped(_ => mockTaskService.Object);

            var serviceProvider = services.BuildServiceProvider();

            var service = new TaskStatusUpdateService(
                serviceProvider,
                mockLogger.Object);

            var cts = new CancellationTokenSource();

            // Act
            // Start the service and cancel after a short delay
            _ = Task.Run(async () =>
            {
                await Task.Delay(100);
                cts.Cancel();
            });

            await service.StartAsync(cts.Token);

            // Assert
            mockTaskService.Verify(
                x => x.UpdateOverdueTasksAsync(),
                Times.AtLeastOnce());
        }
    }
}