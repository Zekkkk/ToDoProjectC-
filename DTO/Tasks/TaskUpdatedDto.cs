namespace ToDo.Api.DTO.Tasks
{
    public class TaskUpdatedDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDateUtc { get; set; }
        public bool IsCompleted { get; set; }
    }
}