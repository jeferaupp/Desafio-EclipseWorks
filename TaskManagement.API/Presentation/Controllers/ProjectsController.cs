using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Interfaces;

namespace TaskManagement.API.Presentation.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProjectAsync([FromBody] Project project)
        {
            if (project == null)
            {
                return BadRequest("Project can not be null.");
            }

            try
            {
                var result = await _projectService.CreateProjectAsync(project);
                return CreatedAtAction(nameof(GetAllProjectsByUserIdAsync), new { userId = project.UserId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllProjectsByUserIdAsync(int userId)
        {
            var projects = await _projectService.GetAllProjectsByUserIdAsync(userId);
            return Ok(projects);
        }
    }
}
