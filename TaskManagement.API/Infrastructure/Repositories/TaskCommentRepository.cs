using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Interfaces;
using TaskManagement.API.Infrastructure.Context;

namespace TaskManagement.API.Infrastructure.Repositories
{
    public class TaskCommentRepository : BaseRepository, ITaskCommentRepository
    {
        public TaskCommentRepository(AppDbContext context) : base(context) { }


        public async Task AddCommentAsync(TaskComment comment)
        {
            await _context.TaskComments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskComment>> GetCommentsByTaskIdAsync(int taskId)
        {
            return await _context.TaskComments.Where(c => c.TaskId == taskId).ToListAsync();
        }
    }

}
