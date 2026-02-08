using ToDo.Api.Domain.Entities;

namespace ToDo.Api.Repository
{
    /// <summary>
    /// USER NEED: Track time logs per task without controllers touching DbContext.
    /// DEV: Interface defines basic CRUD for time tracking operations.
    /// WHY REPO/DTO: Repositories isolate persistence while DTOs define API contracts.
    /// </summary>
    public interface ITimeLogRepository
    {
        Task<TimeLog?> GetByIdAsync(int id);
        Task<List<TimeLog>> GetByTaskIdAsync(int taskItemId);
        Task<TimeLog?> GetRunningByTaskIdAsync(int taskItemId);
        Task<TimeLog> AddAsync(TimeLog timeLog);
        Task UpdateAsync(TimeLog timeLog);
    }
}
