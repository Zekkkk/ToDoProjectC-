namespace ToDo.Api.DTO.TimeLogs
{
    /// <summary>
    /// USER NEED: Read time log details and duration for reporting.
    /// DEV: Response DTO includes computed duration and running state.
    /// WHY REPO/DTO: DTOs keep API contracts stable while repositories isolate persistence.
    /// </summary>
    public class TimeLogResponseDto
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime? EndUtc { get; set; }
        public bool IsRunning { get; set; }
        public int? DurationMinutes { get; set; }
    }
}
