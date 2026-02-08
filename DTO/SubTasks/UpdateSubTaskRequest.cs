using System.ComponentModel.DataAnnotations;

namespace ToDo.Api.DTO.SubTasks
{
    /// <summary>
    /// USER NEED: Provide data to update a checklist item.
    /// DEV: DTO captures editable fields with validation attributes.
    /// WHY REPO/DTO: DTOs keep inputs stable while repositories isolate EF Core access.
    /// </summary>
    public class UpdateSubTaskRequest
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
    }
}
