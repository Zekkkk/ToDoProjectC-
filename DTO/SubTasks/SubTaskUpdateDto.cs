namespace ToDo.Api.DTO.SubTasks
{
    public class SubTaskUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}