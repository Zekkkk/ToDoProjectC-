namespace ToDo.Api.DTO.SubTasks
{
    /// <summary>
    /// USER NEED: Read checklist item details for a task.
    /// DEV: Response DTO includes the minimal fields used by clients.
    /// WHY REPO/DTO: DTOs define the API contract while repositories handle persistence.
    /// </summary>
    public class SubTaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int TaskItemId { get; set; }
    }
}
