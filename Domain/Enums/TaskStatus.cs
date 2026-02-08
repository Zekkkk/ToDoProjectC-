namespace ToDo.Api.Domain.Enums
{
    /// <summary>
    /// USER NEED: Track task progress.
    /// DEV: Enum stored as int by EF Core.
    /// WHY REPO/DTO: DTOs expose allowed values while repositories isolate persistence.
    /// </summary>
    public enum TaskStatus
    {
        Todo = 0,
        InProgress = 1,
        Done = 2
    }
}
