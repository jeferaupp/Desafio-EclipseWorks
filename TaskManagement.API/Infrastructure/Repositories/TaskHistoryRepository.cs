using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Interfaces;
using TaskManagement.API.Infrastructure.Context;

namespace TaskManagement.API.Infrastructure.Repositories
{
    public class TaskHistoryRepository : BaseRepository, ITaskHistoryRepository
    {
        public TaskHistoryRepository(AppDbContext context) : base(context) { }

        public async Task AddHistoryAsync(TaskItemHistory taskHistory)
        {
            _context.TaskItemHistories.Add(taskHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskItemHistory>> GetHistoryByTaskIdAsync(int taskId)
            => await _context.TaskItemHistories.Where(h => h.TaskId == taskId).ToListAsync();
    }
}
