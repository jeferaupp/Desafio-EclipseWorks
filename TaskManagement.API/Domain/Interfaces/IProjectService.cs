using TaskManagement.API.Domain.Entities;

namespace TaskManagement.API.Domain.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjectsByUserIdAsync(int userId);

        Task<Project> CreateProjectAsync(Project project);
    }
}
