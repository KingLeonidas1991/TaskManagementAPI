using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class CreateTaskRequest
    {
        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public TaskStatus Status { get; set; }
    }
}