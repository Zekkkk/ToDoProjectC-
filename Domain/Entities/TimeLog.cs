namespace ToDo.Api.Domain.Entities
{
    /// <summary>
    /// USER NEED: Track time spent on a task (start/stop).
    /// DEV: EndUtc is null while timer is running.
    /// </summary>
    public class TimeLog
    {
        public int Id { get; set; }

        // USER NEED: When I started working.
        public DateTime StartUtc { get; set; }

        // USER NEED: When I stopped working.
        // DEV: null means still running.
        public DateTime? EndUtc { get; set; }

        // ---- Relationship: TaskItem (1) -> (many) TimeLogs ----
        public int TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }

        // USER NEED: Know if the timer is running.
        // DEV: Convenience property (not stored in DB).
        public bool IsRunning => EndUtc == null;
    }
}