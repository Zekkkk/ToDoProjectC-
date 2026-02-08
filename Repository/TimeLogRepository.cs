using Microsoft.EntityFrameworkCore;
using ToDo.Api.Domain.Entities;
using ToDo.Api.Infrastructure.Data;

namespace ToDo.Api.Repository
{
    /// <summary>
    /// USER NEED: Persist time logs for tasks.
    /// DEV: Repository uses AppDbContext and EF Core queries for time log operations.
    /// WHY REPO/DTO: Repositories isolate data access while DTOs define request/response shapes.
    /// </summary>
    public class TimeLogRepository : ITimeLogRepository
    {
        private readonly AppDbContext _db;

        public TimeLogRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TimeLog?> GetByIdAsync(int id)
        {
            return await _db.TimeLogs.FirstOrDefaultAsync(tl => tl.Id == id);
        }

        public async Task<List<TimeLog>> GetByTaskIdAsync(int taskItemId)
        {
            return await _db.TimeLogs
                .AsNoTracking()
                .Where(tl => tl.TaskItemId == taskItemId)
                .OrderByDescending(tl => tl.StartUtc)
                .ToListAsync();
        }

        public async Task<TimeLog?> GetRunningByTaskIdAsync(int taskItemId)
        {
            return await _db.TimeLogs
                .FirstOrDefaultAsync(tl => tl.TaskItemId == taskItemId && tl.EndUtc == null);
        }

        public async Task<TimeLog> AddAsync(TimeLog timeLog)
        {
            _db.TimeLogs.Add(timeLog);
            await _db.SaveChangesAsync();
            return timeLog;
        }

        public async Task UpdateAsync(TimeLog timeLog)
        {
            _db.TimeLogs.Update(timeLog);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TimeLog timeLog)
        {
            _db.TimeLogs.Remove(timeLog);
            await _db.SaveChangesAsync();
        }
    }
}
