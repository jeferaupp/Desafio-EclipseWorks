using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Enums;
using TaskManagement.API.Domain.Interfaces;
using TaskManagement.API.Infrastructure.Context;

namespace TaskManagement.API.Infrastructure.Repositories
{
    public class TaskItemRepository : BaseRepository, ITaskItemRepository
    {
        public TaskItemRepository(AppDbContext context) : base(context) { }

        public async Task AddAsync(TaskItem taskItem)
        {
            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksByProjectIdAsync(int projectId) 
            => await _context.TaskItems.Where(p => p.ProjectId == projectId).ToListAsync();

        public async Task<TaskItem?> GetTaskByIdAsync(int id) => await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id);

        public async Task UpdateAsync(TaskItem task)
        {
            _context.TaskItems.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem task)
        {
            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<TaskItem>> GetCompletedTasksLast30DaysAsync()
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            return await _context.TaskItems
                .Where(task => task.Status == TaskItemStatus.Completed && task.History.Any(h => h.ChangeDate >= thirtyDaysAgo))
                .ToListAsync();
        }
    }
}
