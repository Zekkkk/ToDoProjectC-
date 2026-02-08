namespace ToDo.Api.DTO.SubTasks
{
    public class SubTaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int TaskItemId { get; set; }
    }
}