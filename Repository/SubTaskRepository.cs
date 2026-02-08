using Microsoft.EntityFrameworkCore;
using ToDo.Api.Domain.Entities;
using ToDo.Api.Infrastructure.Data;

namespace ToDo.Api.Repository
{
    /// <summary>
    /// USER NEED: Add and manage checklist items under tasks.
    /// DEV: Repository uses AppDbContext to implement SubTaskItem CRUD.
    /// WHY REPO/DTO: Repositories isolate EF Core access and DTOs define API contracts.
    /// </summary>
    public class SubTaskRepository : ISubTaskRepository
    {
        private readonly AppDbContext _db;

        public SubTaskRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<SubTaskItem?> GetByIdAsync(int id)
        {
            return await _db.SubTasks.FirstOrDefaultAsync(st => st.Id == id);
        }

        public async Task<List<SubTaskItem>> GetByTaskIdAsync(int taskItemId)
        {
            return await _db.SubTasks
                .AsNoTracking()
                .Where(st => st.TaskItemId == taskItemId)
                .ToListAsync();
        }

        public async Task<SubTaskItem> AddAsync(SubTaskItem subTaskItem)
        {
            _db.SubTasks.Add(subTaskItem);
            await _db.SaveChangesAsync();
            return subTaskItem;
        }

        public async Task UpdateAsync(SubTaskItem subTaskItem)
        {
            _db.SubTasks.Update(subTaskItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(SubTaskItem subTaskItem)
        {
            _db.SubTasks.Remove(subTaskItem);
            await _db.SaveChangesAsync();
        }
    }
}
