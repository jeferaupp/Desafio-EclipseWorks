using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Domain.Dtos;
using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Interfaces;

namespace TaskManagement.API.Presentation.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase {

        private readonly ITaskItemService _taskService;

        public TasksController(ITaskItemService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaskAsync([FromBody] TaskItem task)
        {
            if (task == null)
            {
                return BadRequest("Task can not be null.");
            }

            try
            {
                var createdTask = await _taskService.CreateTaskAsync(task);
                return CreatedAtAction(nameof(GetAllTasksByProjectIdAsync), new { projectId = task.ProjectId }, createdTask);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetAllTasksByProjectIdAsync(int projectId)
        {
            var tasks = await _taskService.GetAllTasksByProjectIdAsync(projectId);
            return Ok(tasks);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateTaskAsync(TaskItem task)
        {
            if (task == null)
            {
                return BadRequest("Task can not be null.");
            }

            try
            {
                var updatedTask = await _taskService.UpdateTaskAsync(task);
                return Ok(updatedTask);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch(InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskAsync(int id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id);
                return NoContent(); // 204 NoContent para sucesso de exclusão.
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message); // Tarefa não encontrada.
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Erro interno.
            }
        }

        [HttpGet("performance-report")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPerformanceReportAsync()
        {
            var report = await _taskService.GetPerformanceReportAsync();
            return Ok(report);
        }

        [HttpPost("{taskId}/comments")]
        public async Task<IActionResult> AddCommentToTask(int taskId, [FromBody] AddCommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Comment))
                return BadRequest("Comment cannot be empty.");

            try
            {
                await _taskService.AddCommentAsync(taskId, request.Comment, request.CreatedBy);
                return Ok("Comment added successfully.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
