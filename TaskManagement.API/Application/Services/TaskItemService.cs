using System.Text.Json;
using TaskManagement.API.Domain.Dtos;
using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Interfaces;

namespace TaskManagement.API.Application.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _taskRepository;
        private readonly ITaskCommentRepository _commentRepository;
        private readonly ITaskHistoryRepository _historyRepository;

        public TaskItemService(ITaskItemRepository taskRepository,
            ITaskCommentRepository commentRepository,
            ITaskHistoryRepository historyRepository)
        {
            _taskRepository = taskRepository;
            _commentRepository = commentRepository;
            _historyRepository = historyRepository;
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem newTask)
        {
            var allProjectTasks = await _taskRepository.GetAllTasksByProjectIdAsync(newTask.ProjectId);
            if (allProjectTasks.Count() >= 20)
                throw new InvalidOperationException("Project task limit reached.");

            await _taskRepository.AddAsync(newTask);
            return newTask;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksByProjectIdAsync(int projectId)
        {
            var existingProjectTasks = await _taskRepository.GetAllTasksByProjectIdAsync(projectId);

            if (existingProjectTasks == null)
            {
                return new List<TaskItem>();
            }

            return existingProjectTasks;
        }

        public async Task<TaskItem> UpdateTaskAsync(TaskItem taskItem)
        {
            var existingTask = await _taskRepository.GetTaskByIdAsync(taskItem.Id);
            if(existingTask == null)
            {
                throw new ArgumentException("Task not found.");
            }

            if(taskItem.Priority != existingTask.Priority)
            {
                throw new InvalidDataException("Task priority can not be changed.");
            }

            // Track changes
            var changes = new Dictionary<string, object>();
            if (taskItem.Title != existingTask.Title)
                changes["Title"] = new { Before = existingTask.Title, After = taskItem.Title };

            if (taskItem.Description != existingTask.Description)
                changes["Description"] = new { Before = existingTask.Description, After = taskItem.Description };

            if (taskItem.DueDate != existingTask.DueDate)
                changes["DueDate"] = new { Before = existingTask.DueDate, After = taskItem.DueDate };

            if (taskItem.Status != existingTask.Status)
                changes["Status"] = new { Before = existingTask.Status, After = taskItem.Status };

            existingTask.Title = taskItem.Title ?? existingTask.Title;
            existingTask.Description = taskItem.Description ?? existingTask.Description;
            existingTask.DueDate = taskItem.DueDate ?? existingTask.DueDate;
            existingTask.Status = taskItem.Status ?? existingTask.Status;

            await _taskRepository.UpdateAsync(existingTask);

            // Save history if there are changes
            if (changes.Any())
            {
                var historyEntry = new TaskItemHistory
                {
                    TaskId = taskItem.Id,
                    Changes = JsonSerializer.Serialize(changes),
                    ChangeDate = DateTime.UtcNow
                };

                await _historyRepository.AddHistoryAsync(historyEntry);
            }

            return existingTask;
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                throw new ArgumentException("Task not found.");
            }

            await _taskRepository.DeleteAsync(task);
        }

        public async Task<IEnumerable<UserPerformanceReport>> GetPerformanceReportAsync()
        {
            var completedTasks = await _taskRepository.GetCompletedTasksLast30DaysAsync();

            var groupedByUser = completedTasks
                .GroupBy(task => task.UserId)
                .Select(group => new UserPerformanceReport
                {
                    UserId = group.Key,
                    AverageTasksCompleted = group.Count()
                });

            return groupedByUser;
        }

        public async Task AddCommentAsync(int taskId, string comment, string createdBy)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
                throw new ArgumentException("Task not found.");

            var taskComment = new TaskComment
            {
                TaskId = taskId,
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            await _commentRepository.AddCommentAsync(taskComment);

            var changes = new Dictionary<string, object>();
            changes["Comment"] = new { 
                Action = "Added Comment", 
                Comment = $"Comment added: {comment}"
            };
            var history = new TaskItemHistory
            {
                TaskId = taskId,
                Changes = JsonSerializer.Serialize(changes),
                ChangeDate = DateTime.UtcNow
            };

            await _historyRepository.AddHistoryAsync(history);
        }
    }
}
