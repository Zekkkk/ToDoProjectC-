namespace ToDo.Api.DTO.TimeLogs
{
    /// <summary>
    /// USER NEED: Stop a running time log.
    /// DEV: Optional EndUtc allows client-provided stop time; defaults to now.
    /// WHY REPO/DTO: DTOs define request shape while repositories handle persistence.
    /// </summary>
    public class TimeLogStopDto
    {
        public DateTime? EndUtc { get; set; }
    }
}
