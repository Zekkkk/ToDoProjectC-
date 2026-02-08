using System.ComponentModel.DataAnnotations;

namespace ToDo.Api.Domain.Entities
{
    /// <summary>
    /// USER NEED: Create tasks with deadlines, completion, subtasks, and time tracking.
    /// DEV: TaskItem has relationships to User, SubTasks, and TimeLogs.
    /// </summary>
    public class TaskItem
    {
        public int Id { get; set; }

        // USER NEED: Task must have a title.
        // DEV: Required + length limit.
        [Required]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        // USER NEED: Optional task details.
        // DEV: Nullable + length limit.
        [MaxLength(2000)]
        public string? Description { get; set; }

        // USER NEED: Deadlines.
        // DEV: Stored in UTC for consistent comparisons (Overdue logic later).
        public DateTime? DueDateUtc { get; set; }

        // USER NEED: Mark completed tasks.
        public bool IsCompleted { get; set; }

        // USER NEED: Track when it was completed (useful for reports).
        public DateTime? CompletedAtUtc { get; set; }

        // ---- Relationship: User (1) -> (many) TaskItems ----
        public int UserId { get; set; }
        public User? User { get; set; }

        // ---- Relationship: TaskItem (1) -> (many) SubTasks ----
        public List<SubTask> SubTasks { get; set; } = new();

        // ---- Relationship: TaskItem (1) -> (many) TimeLogs ----
        public List<TimeLog> TimeLogs { get; set; } = new();
    }
}