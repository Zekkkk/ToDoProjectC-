using System.ComponentModel.DataAnnotations;
using ToDo.Api.Domain.Enums;
using TaskStatusEnum = ToDo.Api.Domain.Enums.TaskStatus;

namespace ToDo.Api.Domain.Entities
{
    /// <summary>
    /// USER NEED: Create tasks with title, status, priority, deadlines, and subtasks.
    /// DEV: TaskItem uses enums for status/priority and relates to SubTaskItem and TimeLog.
    /// WHY REPO/DTO: Repositories isolate persistence, and DTOs validate/shape API input/output.
    /// </summary>
    public class TaskItem
    {
        public int Id { get; set; }

        // USER NEED: Task must have a title.
        // DEV: Required + length limit.
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        // USER NEED: Optional task details.
        // DEV: Nullable + length limit.
        [MaxLength(2000)]
        public string? Description { get; set; }

        // USER NEED: Deadlines.
        // DEV: Stored in UTC for consistent comparisons.
        public DateTime? DueDateUtc { get; set; }

        // USER NEED: Priority (Low/Medium/High).
        public TaskPriority Priority { get; set; }

        // USER NEED: Status (Todo/InProgress/Done).
        public TaskStatusEnum Status { get; set; }

        // USER NEED: Track when the task was created.
        public DateTime CreatedAtUtc { get; set; }

        // ---- Relationship: User (1) -> (many) TaskItems ----
        public int UserId { get; set; }
        public User? User { get; set; }

        // ---- Relationship: TaskItem (1) -> (many) SubTaskItems ----
        public List<SubTaskItem> SubTasks { get; set; } = new();

        // ---- Relationship: TaskItem (1) -> (many) TimeLogs ----
        public List<TimeLog> TimeLogs { get; set; } = new();
    }
}
