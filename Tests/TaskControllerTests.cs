using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.Controllers;
using TaskManagement.Models;
using TaskManagement.Services;
using Xunit;
using FluentAssertions;
using TaskStatus = TaskManagement.Models.TaskStatus; // Explicitly use our TaskStatus enum

namespace TaskManagement.Tests
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly TaskController _controller;

        public TaskControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _controller = new TaskController(_mockTaskService.Object);
        }

        [Fact]
        public async Task CreateTask_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createRequest = new CreateTaskRequest
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = TaskStatus.Pending
            };

            var createdTask = new TaskItem
            {
                Id = 1,
                Title = createRequest.Title,
                Description = createRequest.Description,
                DueDate = createRequest.DueDate,
                Status = createRequest.Status
            };

            _mockTaskService.Setup(x => x.CreateTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(createdTask);

            // Act
            var result = await _controller.CreateTask(createRequest);

            // Assert
            var createdAtResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var returnValue = createdAtResult.Value.Should().BeAssignableTo<TaskItem>().Subject;
            returnValue.Id.Should().Be(1);
            returnValue.Title.Should().Be("Test Task");
        }

        [Fact]
        public async Task UpdateTask_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var updateRequest = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Status = TaskStatus.InProgress
            };

            var existingTask = new TaskItem
            {
                Id = id,
                Title = "Original Task",
                Description = "Description",
                DueDate = DateTime.UtcNow,
                Status = TaskStatus.Pending
            };

            _mockTaskService.Setup(x => x.GetTaskByIdAsync(id))
                .ReturnsAsync(existingTask);
            _mockTaskService.Setup(x => x.UpdateTaskAsync(id, It.IsAny<TaskItem>()))
                .ReturnsAsync(existingTask);

            // Act
            var result = await _controller.UpdateTask(id, updateRequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}