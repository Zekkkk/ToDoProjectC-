using System.ComponentModel.DataAnnotations;

namespace ToDo.Api.Domain.Entities
{
    /// <summary>
    /// USER NEED: Split a task into smaller checklist items.
    /// DEV: SubTaskItem belongs to one TaskItem with a required FK.
    /// WHY REPO/DTO: Repositories handle persistence, and DTOs keep request/response shapes clean.
    /// </summary>
    public class SubTaskItem
    {
        public int Id { get; set; }

        // USER NEED: Short checklist title.
        // DEV: Required + length limit.
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        // USER NEED: Mark checklist item completed.
        public bool IsCompleted { get; set; }

        // ---- Relationship: TaskItem (1) -> (many) SubTaskItems ----
        public int TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }
    }
}
