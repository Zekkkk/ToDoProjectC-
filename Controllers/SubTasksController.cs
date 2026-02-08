using Microsoft.AspNetCore.Mvc;
using ToDo.Api.DTO.SubTasks;
using ToDo.Api.Repository;

namespace ToDo.Api.Controllers
{
    /// <summary>
    /// USER NEED: Update or delete checklist items by id.
    /// DEV: Controller validates input then uses repositories for persistence.
    /// WHY REPO/DTO: DTOs define inputs, repositories isolate DbContext access.
    /// </summary>
    [ApiController]
    [Route("api/subtasks")]
    public class SubTasksController : ControllerBase
    {
        private readonly ISubTaskRepository _subTaskRepository;

        public SubTasksController(ISubTaskRepository subTaskRepository)
        {
            _subTaskRepository = subTaskRepository;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSubTask(int id, [FromBody] UpdateSubTaskRequest request)
        {
            var validationError = ValidateSubTaskRequest(request.Title);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var subTaskItem = await _subTaskRepository.GetByIdAsync(id);
            if (subTaskItem == null)
            {
                return NotFound();
            }

            subTaskItem.Title = request.Title.Trim();
            subTaskItem.IsCompleted = request.IsCompleted;

            await _subTaskRepository.UpdateAsync(subTaskItem);

            return Ok(new SubTaskResponseDto
            {
                Id = subTaskItem.Id,
                Title = subTaskItem.Title,
                IsCompleted = subTaskItem.IsCompleted,
                TaskItemId = subTaskItem.TaskItemId
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSubTask(int id)
        {
            var subTaskItem = await _subTaskRepository.GetByIdAsync(id);
            if (subTaskItem == null)
            {
                return NotFound();
            }

            await _subTaskRepository.DeleteAsync(subTaskItem);
            return NoContent();
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
    }
}
