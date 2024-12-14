using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Domain.Entities;

namespace TaskManagement.API.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<TaskItemHistory> TaskItemHistories { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
    }
}