using ToDo.Api.Domain.Entities;
using ToDo.Api.Domain.Enums;
using TaskStatusEnum = ToDo.Api.Domain.Enums.TaskStatus;

namespace ToDo.Api.Repository
{
    /// <summary>
    /// USER NEED: Query and persist tasks without controllers touching EF Core directly.
    /// DEV: Interface defines CRUD plus filtered list operations for TaskItem.
    /// WHY REPO/DTO: Repositories isolate DbContext usage, and DTOs define API contracts.
    /// </summary>
    public interface ITaskRepository
    {
        Task<TaskItem?> GetByIdAsync(int id, int userId);
        Task<bool> ExistsAsync(int id, int userId);
        Task<List<TaskItem>> GetTasksAsync(int userId, TaskStatusEnum? status, TaskPriority? priority, string? query, string? sort);
        Task<TaskItem> AddAsync(TaskItem taskItem);
        Task UpdateAsync(TaskItem taskItem);
        Task DeleteAsync(TaskItem taskItem);
    }
}
