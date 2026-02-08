using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDo.Api.Domain.Entities;
using ToDo.Api.Domain.Enums;
using TaskStatusEnum = ToDo.Api.Domain.Enums.TaskStatus;
using ToDo.Api.DTO.SubTasks;
using ToDo.Api.DTO.Tasks;
using ToDo.Api.Repository;

namespace ToDo.Api.Controllers
{
    /// <summary>
    /// USER NEED: CRUD tasks and manage their subtasks via REST endpoints.
    /// DEV: Controller validates input then calls repository interfaces for persistence.
    /// WHY REPO/DTO: DTOs define request/response shapes while repositories hide DbContext usage.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private static readonly DateTime MinDueDateUtc = new DateTime(2000, 1, 1);
        private readonly ITaskRepository _taskRepository;
        private readonly ISubTaskRepository _subTaskRepository;

        public TasksController(ITaskRepository taskRepository, ISubTaskRepository subTaskRepository)
        {
            _taskRepository = taskRepository;
            _subTaskRepository = subTaskRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var validationError = ValidateTaskRequest(request.Title, request.DueDateUtc);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var taskItem = new TaskItem
            {
                UserId = userId,
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                DueDateUtc = NormalizeDueDate(request.DueDateUtc),
                Priority = request.Priority,
                Status = request.Status,
                CreatedAtUtc = DateTime.UtcNow
            };

            var created = await _taskRepository.AddAsync(taskItem);
            return CreatedAtAction(nameof(GetTaskById), new { id = created.Id }, MapTask(created));
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] string? status, [FromQuery] string? priority, [FromQuery] string? q, [FromQuery] string? sort)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            if (!TryParseStatus(status, out var statusValue, out var statusError))
            {
                return BadRequest(statusError);
            }

            if (!TryParsePriority(priority, out var priorityValue, out var priorityError))
            {
                return BadRequest(priorityError);
            }

            if (!IsValidSort(sort, out var sortError))
            {
                return BadRequest(sortError);
            }

            var tasks = await _taskRepository.GetTasksAsync(userId, statusValue, priorityValue, q?.Trim(), sort);
            var response = tasks.Select(MapTask).ToList();
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var taskItem = await _taskRepository.GetByIdAsync(id, userId);
            if (taskItem == null)
            {
                return NotFound();
            }

            return Ok(MapTask(taskItem));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskRequest request)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var validationError = ValidateTaskRequest(request.Title, request.DueDateUtc);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var taskItem = await _taskRepository.GetByIdAsync(id, userId);
            if (taskItem == null)
            {
                return NotFound();
            }

            taskItem.Title = request.Title.Trim();
            taskItem.Description = request.Description?.Trim();
            taskItem.DueDateUtc = NormalizeDueDate(request.DueDateUtc);
            taskItem.Priority = request.Priority;
            taskItem.Status = request.Status;

            await _taskRepository.UpdateAsync(taskItem);
            return Ok(MapTask(taskItem));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var taskItem = await _taskRepository.GetByIdAsync(id, userId);
            if (taskItem == null)
            {
                return NotFound();
            }

            await _taskRepository.DeleteAsync(taskItem);
            return NoContent();
        }

        [HttpPost("{taskId:int}/subtasks")]
        public async Task<IActionResult> CreateSubTask(int taskId, [FromBody] CreateSubTaskRequest request)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var validationError = ValidateSubTaskRequest(request.Title);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var taskExists = await _taskRepository.ExistsAsync(taskId, userId);
            if (!taskExists)
            {
                return NotFound();
            }

            var subTaskItem = new SubTaskItem
            {
                TaskItemId = taskId,
                Title = request.Title.Trim(),
                IsCompleted = request.IsCompleted
            };

            var created = await _subTaskRepository.AddAsync(subTaskItem);
            return Created($"/api/subtasks/{created.Id}", MapSubTask(created));
        }

        [HttpGet("{taskId:int}/subtasks")]
        public async Task<IActionResult> GetSubTasksForTask(int taskId)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var taskExists = await _taskRepository.ExistsAsync(taskId, userId);
            if (!taskExists)
            {
                return NotFound();
            }

            var subTasks = await _subTaskRepository.GetByTaskIdAsync(taskId);
            var response = subTasks.Select(MapSubTask).ToList();
            return Ok(response);
        }

        private static string? ValidateTaskRequest(string? title, DateTime? dueDateUtc)
        {
            var trimmedTitle = title?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmedTitle))
            {
                return "Title is required.";
            }

            if (trimmedTitle.Length > 100)
            {
                return "Title must be 100 characters or fewer.";
            }

            if (dueDateUtc.HasValue && dueDateUtc.Value < MinDueDateUtc)
            {
                return "DueDateUtc cannot be earlier than 2000-01-01.";
            }

            return null;
        }

        private static string? ValidateSubTaskRequest(string? title)
        {
            var trimmedTitle = title?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmedTitle))
            {
                return "Title is required.";
            }

            if (trimmedTitle.Length > 100)
            {
                return "Title must be 100 characters or fewer.";
            }

            return null;
        }

        private static bool TryParseStatus(string? status, out TaskStatusEnum? statusValue, out string? error)
        {
            statusValue = null;
            error = null;

            if (string.IsNullOrWhiteSpace(status))
            {
                return true;
            }

            if (Enum.TryParse(status, true, out TaskStatusEnum parsed))
            {
                statusValue = parsed;
                return true;
            }

            error = "Invalid status. Allowed values: Todo, InProgress, Done.";
            return false;
        }

        private static bool TryParsePriority(string? priority, out TaskPriority? priorityValue, out string? error)
        {
            priorityValue = null;
            error = null;

            if (string.IsNullOrWhiteSpace(priority))
            {
                return true;
            }

            if (Enum.TryParse(priority, true, out TaskPriority parsed))
            {
                priorityValue = parsed;
                return true;
            }

            error = "Invalid priority. Allowed values: Low, Medium, High.";
            return false;
        }

        private static bool IsValidSort(string? sort, out string? error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(sort))
            {
                return true;
            }

            if (sort.Equals("dueDate", StringComparison.OrdinalIgnoreCase) ||
                sort.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            error = "Invalid sort. Allowed values: dueDate, createdAt.";
            return false;
        }

        private static TaskResponseDto MapTask(TaskItem taskItem)
        {
            return new TaskResponseDto
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                CreatedAtUtc = taskItem.CreatedAtUtc,
                DueDateUtc = taskItem.DueDateUtc,
                Priority = taskItem.Priority,
                Status = taskItem.Status
            };
        }

        private static SubTaskResponseDto MapSubTask(SubTaskItem subTaskItem)
        {
            return new SubTaskResponseDto
            {
                Id = subTaskItem.Id,
                Title = subTaskItem.Title,
                IsCompleted = subTaskItem.IsCompleted,
                TaskItemId = subTaskItem.TaskItemId
            };
        }

        private static DateTime? NormalizeDueDate(DateTime? dueDateUtc)
        {
            if (!dueDateUtc.HasValue)
            {
                return null;
            }

            var date = dueDateUtc.Value.Date;
            return DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claimValue, out userId);
        }
    }
}
