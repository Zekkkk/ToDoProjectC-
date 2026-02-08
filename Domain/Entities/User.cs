using System.ComponentModel.DataAnnotations;

namespace ToDo.Api.Domain.Entities
{
    /// <summary>
    /// USER NEED: A person who owns tasks.
    /// DEV: Store login identity and connect tasks to this user (1-to-many).
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        // USER NEED: A unique name to login with.
        // DEV: Required + limited length for database safety.
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        // USER NEED: Password must be secure.
        // DEV: Store only a HASH (never store plain password).
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // USER NEED: User can create many tasks.
        // DEV: Navigation property for 1-to-many relationship.
        public List<TaskItem> Tasks { get; set; } = new();
    }
}