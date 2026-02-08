using System.ComponentModel.DataAnnotations;

namespace ToDo.Api.Domain.Entities
{
    /// <summary>
    /// USER NEED: Split a big task into smaller steps.
    /// DEV: SubTask belongs to exactly one TaskItem.
    /// </summary>
    public class SubTask
    {
        public int Id { get; set; }

        // USER NEED: A short subtask title.
        [Required]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        // USER NEED: Mark subtask completed.
        public bool IsCompleted { get; set; }

        // ---- Relationship: TaskItem (1) -> (many) SubTasks ----
        public int TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }
    }
}