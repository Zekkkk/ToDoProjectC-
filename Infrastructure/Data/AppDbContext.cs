using Microsoft.EntityFrameworkCore;
using ToDo.Api.Domain.Entities;

namespace ToDo.Api.Infrastructure.Data
{
    /// <summary>
    /// USER NEED: Persist tasks/users to a real database.
    /// DEV: EF Core DbContext maps our Domain entities to PostgreSQL tables.
    /// </summary>
    public class AppDbContext : DbContext
    {
        // DbContextOptions comes from DI in Program.cs
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Each DbSet becomes a table in PostgreSQL.
        public DbSet<User> Users => Set<User>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();
        public DbSet<SubTask> SubTasks => Set<SubTask>();
        public DbSet<TimeLog> TimeLogs => Set<TimeLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER NEED: Username must be unique (no duplicate accounts).
            // DEV: Add unique index.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // USER NEED: One user has many tasks.
            // DEV: Configure FK and cascade delete tasks when user is deleted.
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // USER NEED: One task has many subtasks.
            modelBuilder.Entity<SubTask>()
                .HasOne(st => st.TaskItem)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(st => st.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // USER NEED: One task has many time logs.
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
