namespace ToDo.Api.DTO.TimeLogs
{
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