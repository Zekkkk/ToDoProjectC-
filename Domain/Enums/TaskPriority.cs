namespace ToDo.Api.Domain.Enums
{
    /// <summary>
    /// USER NEED: Rank task urgency.
    /// DEV: Enum stored as int by EF Core.
    /// WHY REPO/DTO: DTOs expose allowed values while repositories isolate persistence.
    /// </summary>
    public enum TaskPriority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
}
