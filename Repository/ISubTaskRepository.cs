using ToDo.Api.Domain.Entities;

namespace ToDo.Api.Repository
{
    /// <summary>
    /// USER NEED: Manage checklist items without direct DbContext access.
    /// DEV: Interface defines CRUD operations for SubTaskItem.
    /// WHY REPO/DTO: Repositories isolate persistence; DTOs define request/response shapes.
    /// </summary>
    public interface ISubTaskRepository
    {
        Task<SubTaskItem?> GetByIdAsync(int id);
        Task<List<SubTaskItem>> GetByTaskIdAsync(int taskItemId);
        Task<SubTaskItem> AddAsync(SubTaskItem subTaskItem);
        Task UpdateAsync(SubTaskItem subTaskItem);
        Task DeleteAsync(SubTaskItem subTaskItem);
    }
}
