using TaskManagement.API.Domain.Entities;

namespace TaskManagement.API.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project);
        Task<IEnumerable<Project>> GetAllProjectsByUserIdAsync(int userId);
    }
}
