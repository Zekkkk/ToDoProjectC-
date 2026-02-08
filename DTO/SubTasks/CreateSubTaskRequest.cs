using System.ComponentModel.DataAnnotations;

namespace ToDo.Api.DTO.SubTasks
{
    /// <summary>
    /// USER NEED: Provide data to create a checklist item.
    /// DEV: DTO captures required fields with validation attributes.
    /// WHY REPO/DTO: DTOs keep API inputs clean while repositories handle data access.
    /// </summary>
    public class CreateSubTaskRequest
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
    }
}
