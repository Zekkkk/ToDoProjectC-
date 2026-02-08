using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDo.Api.Domain.Entities;
using ToDo.Api.DTO.TimeLogs;
using ToDo.Api.Repository;

namespace ToDo.Api.Controllers
{
    /// <summary>
    /// USER NEED: Start, stop, and view time logs for tasks.
    /// DEV: Controller validates input and uses repositories to persist and query data.
    /// WHY REPO/DTO: DTOs define request/response shapes while repositories isolate DbContext access.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/tasks/{taskId:int}/timelogs")]
    public class TimeLogsController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITimeLogRepository _timeLogRepository;

        public TimeLogsController(ITaskRepository taskRepository, ITimeLogRepository timeLogRepository)
        {
            _taskRepository = taskRepository;
            _timeLogRepository = timeLogRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeLogsForTask(int taskId)
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

            var logs = await _timeLogRepository.GetByTaskIdAsync(taskId);
            var response = logs.Select(MapTimeLog).ToList();
            return Ok(response);
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartTimeLog(int taskId, [FromBody] TimeLogStartDto dto)
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

            var runningLog = await _timeLogRepository.GetRunningByTaskIdAsync(taskId);
            if (runningLog != null)
            {
                return BadRequest("Task already has a running time log.");
            }

            var startUtc = dto.StartUtc ?? DateTime.UtcNow;

            var timeLog = new TimeLog
            {
                TaskItemId = taskId,
                StartUtc = startUtc,
                EndUtc = null
            };

            var created = await _timeLogRepository.AddAsync(timeLog);
            return CreatedAtAction(nameof(GetTimeLogsForTask), new { taskId }, MapTimeLog(created));
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopTimeLog(int taskId, [FromBody] TimeLogStopDto dto)
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

            var runningLog = await _timeLogRepository.GetRunningByTaskIdAsync(taskId);
            if (runningLog == null)
            {
                return NotFound();
            }

            var endUtc = dto.EndUtc ?? DateTime.UtcNow;
            if (endUtc < runningLog.StartUtc)
            {
                return BadRequest("EndUtc cannot be earlier than StartUtc.");
            }

            runningLog.EndUtc = endUtc;
            await _timeLogRepository.UpdateAsync(runningLog);

            return Ok(MapTimeLog(runningLog));
        }

        private static TimeLogResponseDto MapTimeLog(TimeLog timeLog)
        {
            int? durationMinutes = null;
            if (timeLog.EndUtc.HasValue)
            {
                durationMinutes = (int)Math.Floor((timeLog.EndUtc.Value - timeLog.StartUtc).TotalMinutes);
            }

            return new TimeLogResponseDto
            {
                Id = timeLog.Id,
                TaskItemId = timeLog.TaskItemId,
                StartUtc = timeLog.StartUtc,
                EndUtc = timeLog.EndUtc,
                IsRunning = timeLog.IsRunning,
                DurationMinutes = durationMinutes
            };
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claimValue, out userId);
        }
    }
}
