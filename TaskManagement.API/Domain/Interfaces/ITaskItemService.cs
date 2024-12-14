using TaskManagement.API.Domain.Dtos;
using TaskManagement.API.Domain.Entities;

namespace TaskManagement.API.Domain.Interfaces
{
    public interface ITaskItemService
    {
        Task<TaskItem> CreateTaskAsync(TaskItem newTask);
        Task DeleteTaskAsync(int taskId);
        Task<IEnumerable<TaskItem>> GetAllTasksByProjectIdAsync(int projectId);
        Task<TaskItem> UpdateTaskAsync(TaskItem taskItem);
        Task<IEnumerable<UserPerformanceReport>> GetPerformanceReportAsync();
        Task AddCommentAsync(int taskId, string comment, string createdBy);
    }
}
