using System.ComponentModel.DataAnnotations;
using ToDo.Api.Domain.Enums;
using TaskStatusEnum = ToDo.Api.Domain.Enums.TaskStatus;

namespace ToDo.Api.DTO.Tasks
{
    /// <summary>
    /// USER NEED: Provide data to update an existing task.
    /// DEV: DTO mirrors editable fields and enforces validation attributes.
    /// WHY REPO/DTO: DTOs prevent over-posting while repositories isolate persistence.
    /// </summary>
    public class UpdateTaskRequest
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public DateTime? DueDateUtc { get; set; }

        public TaskPriority Priority { get; set; }

        public TaskStatusEnum Status { get; set; }
    }
}
