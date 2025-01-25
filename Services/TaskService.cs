using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;
using Microsoft.Extensions.Logging;

namespace TaskManagement.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext _context;
        private readonly ILogger<TaskService> _logger;

        public TaskService(TaskDbContext context, ILogger<TaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            task.CreatedAt = DateTime.UtcNow;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem?> UpdateTaskAsync(int id, TaskItem task)
        {
            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
                return null;

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Status = task.Status;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingTask;
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateOverdueTasksAsync()
        {
            _logger.LogInformation("Starting to update overdue tasks");

            var currentDate = DateTime.UtcNow;
            var tasksToUpdate = await _context.Tasks
                .Where(t => t.DueDate < currentDate &&
                           (t.Status == Models.TaskStatus.Pending ||
                            t.Status == Models.TaskStatus.InProgress))
                .ToListAsync();

            int updatedCount = 0;
            foreach (var task in tasksToUpdate)
            {
                if (task.Status == Models.TaskStatus.Pending)
                {
                    task.Status = Models.TaskStatus.Overdue;
                    _logger.LogInformation("Task {TaskId} status changed from Pending to Overdue", task.Id);
                    updatedCount++;
                }
                else if (task.Status == Models.TaskStatus.InProgress)
                {
                    task.Status = Models.TaskStatus.Completed;
                    _logger.LogInformation("Task {TaskId} status changed from InProgress to Completed", task.Id);
                    updatedCount++;
                }
                task.UpdatedAt = currentDate;
            }

            if (updatedCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated {Count} tasks", updatedCount);
            }
        }
    }
}