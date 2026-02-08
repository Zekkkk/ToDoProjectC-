namespace ToDo.Api.DTO.Tasks
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDateUtc { get; set; } 
        public bool IsCompleted { get; set; }
        public DateTime? CompletedOnUtc { get; set; }
        
        // useful computed field for ui and reports
        public bool IsOverdue { get; set; }
    }
}