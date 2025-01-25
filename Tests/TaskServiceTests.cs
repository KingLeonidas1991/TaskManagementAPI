using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Services;
using Xunit;
using FluentAssertions;
using Moq;
using TaskStatus = TaskManagement.Models.TaskStatus; // Explicitly use our TaskStatus enum

namespace TaskManagement.Tests
{
    public class TaskServiceTests
    {
        private readonly TaskDbContext _context;
        private readonly TaskService _taskService;
        private readonly ILogger<TaskService> _logger;

        public TaskServiceTests()
        {
            var options = new DbContextOptionsBuilder<TaskDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TaskDbContext(options);
            var mockLogger = new Mock<ILogger<TaskService>>();
            _logger = mockLogger.Object;
            _taskService = new TaskService(_context, _logger);
        }

        [Fact]
        public async Task CreateTask_ShouldCreateNewTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = TaskStatus.Pending
            };

            // Act
            var result = await _taskService.CreateTaskAsync(task);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Title.Should().Be("Test Task");
        }

        [Fact]
        public async Task UpdateOverdueTasks_ShouldUpdatePendingToOverdue()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "Overdue Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(-1),
                Status = TaskStatus.Pending
            };
            await _taskService.CreateTaskAsync(task);

            // Act
            await _taskService.UpdateOverdueTasksAsync();

            // Assert
            var updatedTask = await _taskService.GetTaskByIdAsync(task.Id);
            updatedTask.Should().NotBeNull();
            updatedTask!.Status.Should().Be(TaskStatus.Overdue);
        }

        [Fact]
        public async Task UpdateOverdueTasks_ShouldUpdateInProgressToCompleted()
        {
            // Arrange
            var task = new TaskItem
            {
                Title = "In Progress Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(-1),
                Status = TaskStatus.InProgress
            };
            await _taskService.CreateTaskAsync(task);

            // Act
            await _taskService.UpdateOverdueTasksAsync();

            // Assert
            var updatedTask = await _taskService.GetTaskByIdAsync(task.Id);
            updatedTask.Should().NotBeNull();
            updatedTask!.Status.Should().Be(TaskStatus.Completed);
        }
    }
}