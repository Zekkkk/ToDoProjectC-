namespace ToDo.Api.DTO.Tasks
{
    public class TaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDateUtc { get; set; }
    }
}