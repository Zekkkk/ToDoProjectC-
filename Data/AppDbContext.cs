using Microsoft.EntityFrameworkCore;
using ToDo.Api.Domain.Entities;

namespace ToDo.Api.Infrastructure.Data
{
    /// <summary>
    /// USER NEED: Persist tasks, subtasks, and users in PostgreSQL.
    /// DEV: EF Core DbContext maps domain entities and configures relationships/cascade rules.
    /// WHY REPO/DTO: Repositories isolate DbContext access, and DTOs define stable API contracts.
    /// </summary>
    public class AppDbContext : DbContext
    {
        // DbContextOptions comes from DI in Program.cs
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Each DbSet becomes a table in PostgreSQL.
        public DbSet<User> Users => Set<User>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();
        public DbSet<SubTaskItem> SubTasks => Set<SubTaskItem>();
        public DbSet<TimeLog> TimeLogs => Set<TimeLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER NEED: Username must be unique (no duplicate accounts).
            // DEV: Add unique index.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // USER NEED: Tasks can exist without auth for now.
            // DEV: Configure nullable FK so CRUD works before JWT is added.
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // USER NEED: One task has many subtasks.
            // DEV: Configure FK and cascade delete.
            modelBuilder.Entity<SubTaskItem>()
                .HasOne(st => st.TaskItem)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(st => st.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // USER NEED: One task has many time logs.
            // DEV: Configure FK and cascade delete.
            modelBuilder.Entity<TimeLog>()
                .HasOne(tl => tl.TaskItem)
                .WithMany(t => t.TimeLogs)
                .HasForeignKey(tl => tl.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // USER NEED: StartUtc is required for time tracking.
            // DEV: Ensure it is required at DB level.
            modelBuilder.Entity<TimeLog>()
                .Property(tl => tl.StartUtc)
                .IsRequired();
        }
    }
}
