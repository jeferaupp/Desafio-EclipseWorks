using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Interfaces;

namespace TaskManagement.API.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            await _projectRepository.AddAsync(project);
            return project;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsByUserIdAsync(int userId)
        {
            var existingProjects = await _projectRepository.GetAllProjectsByUserIdAsync(userId);

            if(existingProjects == null)
            {
                return new List<Project>();
            }

            return existingProjects;
        }
    }
}
