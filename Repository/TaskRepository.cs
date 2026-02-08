using Microsoft.EntityFrameworkCore;
using ToDo.Api.Domain.Entities;
using ToDo.Api.Domain.Enums;
using TaskStatusEnum = ToDo.Api.Domain.Enums.TaskStatus;
using ToDo.Api.Infrastructure.Data;

namespace ToDo.Api.Repository
{
    /// <summary>
    /// USER NEED: Retrieve and store tasks with filters and sorting.
    /// DEV: Repository uses AppDbContext and EF Core query composition.
    /// WHY REPO/DTO: Controllers stay thin while DTOs validate inputs.
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _db;

        public TaskRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TaskItem?> GetByIdAsync(int id, int userId)
        {
            return await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<bool> ExistsAsync(int id, int userId)
        {
            return await _db.TaskItems.AnyAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<List<TaskItem>> GetTasksAsync(int userId, TaskStatusEnum? status, TaskPriority? priority, string? query, string? sort)
        {
            IQueryable<TaskItem> taskQuery = _db.TaskItems.AsNoTracking()
                .Where(t => t.UserId == userId);

            if (status.HasValue)
            {
                taskQuery = taskQuery.Where(t => t.Status == status.Value);
            }

            if (priority.HasValue)
            {
                taskQuery = taskQuery.Where(t => t.Priority == priority.Value);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                taskQuery = taskQuery.Where(t => EF.Functions.ILike(t.Title, $"%{query}%"));
            }

            taskQuery = sort?.ToLowerInvariant() switch
            {
                "duedate" => taskQuery.OrderBy(t => t.DueDateUtc ?? DateTime.MaxValue),
                "createdat" => taskQuery.OrderBy(t => t.CreatedAtUtc),
                _ => taskQuery
            };

            return await taskQuery.ToListAsync();
        }

        public async Task<TaskItem> AddAsync(TaskItem taskItem)
        {
            _db.TaskItems.Add(taskItem);
            await _db.SaveChangesAsync();
            return taskItem;
        }

        public async Task UpdateAsync(TaskItem taskItem)
        {
            _db.TaskItems.Update(taskItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem taskItem)
        {
            _db.TaskItems.Remove(taskItem);
            await _db.SaveChangesAsync();
        }
    }
}
