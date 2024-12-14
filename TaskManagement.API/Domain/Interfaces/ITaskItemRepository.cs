using TaskManagement.API.Domain.Entities;

namespace TaskManagement.API.Domain.Interfaces
{
    public interface ITaskItemRepository
    {
        Task AddAsync(TaskItem task);
        Task<IEnumerable<TaskItem>> GetAllTasksByProjectIdAsync(int projectId);
        Task<TaskItem> GetTaskByIdAsync(int id);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task<IEnumerable<TaskItem>> GetCompletedTasksLast30DaysAsync();
    }
}
