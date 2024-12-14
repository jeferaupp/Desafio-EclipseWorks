using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Interfaces;
using TaskManagement.API.Infrastructure.Context;

namespace TaskManagement.API.Infrastructure.Repositories
{

    public class ProjectRepository : BaseRepository, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context) { }

        public async Task AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Project>> GetAllProjectsByUserIdAsync(int userId) 
            => await _context.Projects.Where(p => p.UserId == userId).ToListAsync();
    }

}
