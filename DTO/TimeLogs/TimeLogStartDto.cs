namespace ToDo.Api.DTO.TimeLogs
{
    /// <summary>
    /// USER NEED: Start a time log for a task.
    /// DEV: Optional StartUtc allows client-provided start time; defaults to now.
    /// WHY REPO/DTO: DTOs define request shape while repositories handle persistence.
    /// </summary>
    public class TimeLogStartDto
    {
        // optional note later, but keep minimal for exam
        public DateTime? StartUtc { get; set; }
    }
}
