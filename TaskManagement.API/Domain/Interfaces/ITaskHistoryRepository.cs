using TaskManagement.API.Domain.Entities;

namespace TaskManagement.API.Domain.Interfaces
{
    public interface ITaskHistoryRepository
    {
        Task AddHistoryAsync(TaskItemHistory taskHistory);
        Task<IEnumerable<TaskItemHistory>> GetHistoryByTaskIdAsync(int taskId);
    }
}
