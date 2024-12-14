using Moq;
using TaskManagement.API.Application.Services;
using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Enums;
using TaskManagement.API.Domain.Interfaces;

namespace TaskManagement.Tests.Application.Services
{
    public class TaskServiceTest
    {
        private Mock<ITaskItemRepository> _mockTaskRepository;
        private Mock<ITaskCommentRepository> _mockCommentRepository;
        private Mock<ITaskHistoryRepository> _mockHistoryRepository;
        private ITaskItemService _taskService;

        public TaskServiceTest()
        {
            _mockTaskRepository = new Mock<ITaskItemRepository>();
            _mockCommentRepository = new Mock<ITaskCommentRepository>();
            _mockHistoryRepository = new Mock<ITaskHistoryRepository>();
            _taskService = new TaskItemService(_mockTaskRepository.Object, _mockCommentRepository.Object, _mockHistoryRepository.Object);
        }

        [Theory]
        [MemberData(nameof(GetAllTasksByProjectId_ValidResults))]
        public async Task GetAllTasksByProjectId_ValidProjectId_ReturnsTasksAsync(List<TaskItem> tasks, int tasksCount)
        {
            // Arrange
            _mockTaskRepository.Setup(repo => repo.GetAllTasksByProjectIdAsync(It.IsAny<int>())).ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasksByProjectIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tasksCount, result.Count());
            Assert.Equal(tasks, result);
        }

        public static IEnumerable<object[]> GetAllTasksByProjectId_ValidResults =>
            new List<object[]>
            {
                new object[]
                {
                    new List<TaskItem> {
                        new TaskItem { Id = 1, Title = "Task 1", ProjectId = 1, Status = TaskItemStatus.Pending },
                        new TaskItem { Id = 2, Title = "Task 2", ProjectId = 1, Status = TaskItemStatus.Completed }
                    },
                    2
                },
                new object[]
                {
                    new List<TaskItem> (),
                    0
                }
            };

        [Fact]
        public async Task CreateTaskAsync_ValidProject_ReturnsCreatedTaskAsync()
        {
            // Arrange
            var newTask = new TaskItem { Title = "Task 1", ProjectId = 1, Status = TaskItemStatus.Pending };
            var mockCreatedTask = newTask;
            mockCreatedTask.Id = 1;

            _mockTaskRepository.Setup(repo => repo.AddAsync(It.IsAny<TaskItem>()));

            // Act
            var result = await _taskService.CreateTaskAsync(newTask);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockCreatedTask.Id, result.Id);
        }

        [Fact]
        public void CreateTask_ProjectTaskLimitReached_ThrowsInvalidOperationException()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(5),
                Status = TaskItemStatus.Pending,
                Priority = PriorityLevel.Medium,
                ProjectId = 1
            };

            // Simula que o projeto já tem 20 tarefas
            _mockTaskRepository.Setup(repo => repo.GetAllTasksByProjectIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<TaskItem>(new TaskItem[20]));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _taskService.CreateTaskAsync(task));
            Assert.Equal("Project task limit reached.", ex.Result.Message);
        }

        [Fact]
        public async Task UpdateTask_ValidTask_ReturnsUpdatedTaskAsync()
        {
            // Arrange
            var taskId = 1;
            var existingTask = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(5),
                Status = TaskItemStatus.Pending,
                Priority = PriorityLevel.Medium,
                ProjectId = 1
            };
            var updatedTask = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Updated Description",
                DueDate = DateTime.Now.AddDays(5),
                Status = TaskItemStatus.Pending,
                Priority = PriorityLevel.Medium,
                ProjectId = 1
            };

            _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync(existingTask);
            _mockTaskRepository.Setup(repo => repo.UpdateAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
            _mockHistoryRepository.Setup(repo => repo.AddHistoryAsync(It.IsAny<TaskItemHistory>())).Returns(Task.CompletedTask);

            // Act 
            var result = await _taskService.UpdateTaskAsync(updatedTask);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedTask.Description, result.Description);
            Assert.Equal(updatedTask.Description, result.Description);
            Assert.Equal(updatedTask.Status, result.Status);
            Assert.Equal(updatedTask.Priority, result.Priority);

            _mockTaskRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
            _mockHistoryRepository.Verify(repo => repo.AddHistoryAsync(It.IsAny<TaskItemHistory>()), Times.Once);

        }

        [Fact]
        public async Task UpdateTask_InvalidValidTask_ThrowsArgumentExceptionTaskAsync()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(5),
                Status = TaskItemStatus.Pending,
                Priority = PriorityLevel.Medium,
                ProjectId = 1
            };

            _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(It.IsAny<int>())).ReturnsAsync((TaskItem)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _taskService.UpdateTaskAsync(task));
            Assert.Equal("Task not found.", ex.Result.Message);
        }



        [Fact]
        public async Task UpdateTask_ChangePriorityTask_ThrowsInvalidDataExceptionTaskAsync()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(5),
                Status = TaskItemStatus.Pending,
                Priority = PriorityLevel.Medium,
                ProjectId = 1
            };
            var updatedTask = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(5),
                Status = TaskItemStatus.Pending,
                Priority = PriorityLevel.High,
                ProjectId = 1
            };

            _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(It.IsAny<int>())).ReturnsAsync(task);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidDataException>(() => _taskService.UpdateTaskAsync(updatedTask));
            Assert.Equal("Task priority can not be changed.", ex.Result.Message);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldCallDelete_WhenTaskExists()
        {
            // Arrange
            var taskId = 1;
            var task = new TaskItem { Id = taskId, Title = "Sample Task" };

            _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync(task);
            _mockTaskRepository.Setup(repo => repo.DeleteAsync(task)).Returns(Task.CompletedTask);

            // Act
            await _taskService.DeleteTaskAsync(taskId);

            // Assert
            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(repo => repo.DeleteAsync(task), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldThrowArgumentException_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = 99;

            _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync((TaskItem)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _taskService.DeleteTaskAsync(taskId));
            Assert.Equal("Task not found.", exception.Message);

            // Ensure GetTaskByIdAsync was called but DeleteAsync was not
            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(repo => repo.DeleteAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var taskId = 1;
            var task = new TaskItem { Id = taskId, Title = "Sample Task" };

            _mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync(task);
            _mockTaskRepository.Setup(repo => repo.DeleteAsync(task)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _taskService.DeleteTaskAsync(taskId));
            Assert.Equal("Database error", exception.Message);

            // Ensure both methods were called
            _mockTaskRepository.Verify(repo => repo.GetTaskByIdAsync(taskId), Times.Once);
            _mockTaskRepository.Verify(repo => repo.DeleteAsync(task), Times.Once);
        }

        [Fact]
        public async Task GetPerformanceReportAsync_ShouldReturnAverageTasksPerUser()
        {
            // Arrange
            var mockTasks = new List<TaskItem>
                {
                    new TaskItem {
                        UserId = 1,
                        Status = TaskItemStatus.Completed,
                        History = new List<TaskItemHistory> () {
                            new TaskItemHistory { ChangeDate = DateTime.UtcNow }
                        }
                    },
                    new TaskItem {
                        UserId = 1,
                        Status = TaskItemStatus.Completed,
                        History = new List<TaskItemHistory> () {
                            new TaskItemHistory { ChangeDate = DateTime.UtcNow }
                        }
                    },
                    new TaskItem {
                        UserId = 2,
                        Status = TaskItemStatus.Completed,
                        History = new List<TaskItemHistory> () {
                            new TaskItemHistory { ChangeDate = DateTime.UtcNow }
                        }
                    },
                };
            _mockTaskRepository.Setup(repo => repo.GetCompletedTasksLast30DaysAsync())
                .ReturnsAsync(mockTasks);

            // Act
            var result = await _taskService.GetPerformanceReportAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.UserId == 1 && r.AverageTasksCompleted == 2);
            Assert.Contains(result, r => r.UserId == 2 && r.AverageTasksCompleted == 1);
        }

        [Fact]
        public async Task AddCommentAsync_ShouldAddCommentAndRegisterHistory()
        {
            // Arrange
            var mockTaskRepository = new Mock<ITaskItemRepository>();
            var mockCommentRepository = new Mock<ITaskCommentRepository>();
            var mockHistoryRepository = new Mock<ITaskHistoryRepository>();

            var taskId = 1;
            var task = new TaskItem { Id = taskId, Title = "Sample Task" };

            mockTaskRepository.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync(task);

            var service = new TaskItemService(mockTaskRepository.Object, mockCommentRepository.Object, mockHistoryRepository.Object);

            // Act
            await service.AddCommentAsync(taskId, "This is a comment.", "User1");

            // Assert
            mockCommentRepository.Verify(repo => repo.AddCommentAsync(It.IsAny<TaskComment>()), Times.Once);
            mockHistoryRepository.Verify(repo => repo.AddHistoryAsync(It.IsAny<TaskItemHistory>()), Times.Once);
        }
    }
}
