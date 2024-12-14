using TaskManagement.API.Domain.Entities;

namespace TaskManagement.API.Domain.Interfaces
{
    public interface ITaskCommentRepository
    {
        Task AddCommentAsync(TaskComment comment);
        Task<IEnumerable<TaskComment>> GetCommentsByTaskIdAsync(int taskId);
    }
}
