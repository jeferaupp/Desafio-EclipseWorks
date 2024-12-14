using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.API.Application.Services;
using TaskManagement.API.Domain.Dtos;
using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Enums;
using TaskManagement.API.Domain.Interfaces;
using TaskManagement.API.Presentation.Controllers;

namespace TaskManagement.Tests.Presentation.Controllers
{
    public class TasksControllerTests
    {
        [Fact]
        public async Task GetAllTasksByProjectIdAsync_ShouldReturnListOfTasksAsync()
        {
            // Arrange
            var mockService = new Mock<ITaskItemService>();
            var projectId = 1;
            var mockTasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Task 1", ProjectId = projectId, Status = TaskItemStatus.Pending },
                new TaskItem { Id = 2, Title = "Task 2", ProjectId = projectId, Status = TaskItemStatus.Completed }
            };
            mockService.Setup(service => service.GetAllTasksByProjectIdAsync(projectId)).ReturnsAsync(mockTasks);

            var controller = new TasksController(mockService.Object);

            // Act
            var result = await controller.GetAllTasksByProjectIdAsync(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProjects = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(okResult.Value);
            Assert.NotNull(returnedProjects);
            Assert.Equal(2, returnedProjects.Count());
            Assert.Contains(returnedProjects, p => p.Title == "Task 1");
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldReturnCreatedTaskAsync()
        {
            // Arrange
            var mockService = new Mock<ITaskItemService>();
            var projectId = 1;
            var newTask = new TaskItem { Title = "Task 1", ProjectId = projectId, Status = TaskItemStatus.Pending };
            var mockCreatedTask = newTask;
            mockCreatedTask.Id = 1;
            mockService.Setup(service => service.CreateTaskAsync(It.IsAny<TaskItem>())).ReturnsAsync(newTask);

            var controller = new TasksController(mockService.Object);

            // Act
            var result = await controller.CreateTaskAsync(newTask);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedTask = Assert.IsAssignableFrom<TaskItem>(createdResult.Value);
            Assert.NotNull(returnedTask);
            Assert.Equal(mockCreatedTask.Id, returnedTask.Id);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnUpdatedTaskAsync()
        {
            // Arrange
            var mockService = new Mock<ITaskItemService>();
            var mockUpdatedTask = new TaskItem { Id = 1, ProjectId = 1, Title = "Task 1", Status = TaskItemStatus.Pending, Description = "Update Task 1 Description" };
            mockService.Setup(service => service.UpdateTaskAsync(It.IsAny<TaskItem>())).ReturnsAsync(mockUpdatedTask);

            var controller = new TasksController(mockService.Object);

            // Act
            var result = await controller.UpdateTaskAsync(mockUpdatedTask);

            // Assert
            var updatedResult = Assert.IsType<OkObjectResult>(result);
            var returnedTask = Assert.IsAssignableFrom<TaskItem>(updatedResult.Value);
            Assert.NotNull(returnedTask);
            Assert.Equal("Update Task 1 Description", returnedTask.Description);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnBadRequest_WhenTaskIsNull()
        {
            // Arrange
            var mockService = new Mock<ITaskItemService>();
            var controller = new TasksController(mockService.Object);

            // Act
            var result = await controller.UpdateTaskAsync(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Task can not be null.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnNotFound_WhenArgumentExceptionIsThrown()
        {
            // Arrange
            var mockService = new Mock<ITaskItemService>();
            var invalidTask = new TaskItem { Id = 99, Title = "Invalid Task" };
            mockService.Setup(service => service.UpdateTaskAsync(invalidTask))
                .ThrowsAsync(new ArgumentException("Task not found."));

            var controller = new TasksController(mockService.Object);

            // Act
            var result = await controller.UpdateTaskAsync(invalidTask);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Task not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnBadRequest_WhenInvalidDataExceptionIsThrown()
        {
            // Arrange
            var mockService = new Mock<ITaskItemService>();
            var invalidTask = new TaskItem { Id = 1, Title = "" };
            mockService.Setup(service => service.UpdateTaskAsync(invalidTask))
                .ThrowsAsync(new InvalidDataException("Invalid task data."));

            var controller = new TasksController(mockService.Object);

            // Act
            var result = await controller.UpdateTaskAsync(invalidTask);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid task data.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnInternalServerError_WhenGenericExceptionIsThrown()
        {
            // Arrange
            var mockService = new Mock<ITaskItemService>();
            var task = new TaskItem { Id = 1, Title = "Task with Error" };
            mockService.Setup(service => service.UpdateTaskAsync(task))
                .ThrowsAsync(new Exception("Something went wrong."));

            var controller = new TasksController(mockService.Object);

            // Act
            var result = await controller.UpdateTaskAsync(task);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Something went wrong.", statusCodeResult.Value);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldReturnNoContent_WhenTaskIsDeletedSuccessfully()
        {
            // Arrange
            var mockService = new Mock<ITaskItemService>();
            var taskId = 1;

            mockService.Setup(service => service.DeleteTaskAsync(taskId)).Returns(Task.CompletedTask);

            var controller = new TasksController(mockService.Object);

            // Act
            var result = await controller.DeleteTaskAsync(taskId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldReturnNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var mockService = new Mock<ITaskItemService>();
            var taskId = 99;
            var errorMessage = "Task not found.";

            mockService.Setup(service => service.DeleteTaskAsync(taskId))
                .ThrowsAsync(new ArgumentException(errorMessage));

            var controller = new TasksController(mockService.Object);

            // Act
            var result = await controller.DeleteTaskAsync(taskId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(errorMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldReturnInternalServerError_WhenAnExceptionOccurs()
        {
            // Arrange
            var mockService = new Mock<ITaskItemService>();
            var taskId = 1;
            var errorMessage = "An unexpected error occurred.";

            mockService.Setup(service => service.DeleteTaskAsync(taskId))
                .ThrowsAsync(new Exception(errorMessage));

            var controller = new TasksController(mockService.Object);

            // Act
            var result = await controller.DeleteTaskAsync(taskId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal(errorMessage, statusCodeResult.Value);
        }

        [Fact]
        public async Task GetPerformanceReportAsync_ShouldReturnAverageTasksPerUser()
        {
            // Arrange
            var mockRepository = new Mock<ITaskItemRepository>();
            var mockCommentRepository = new Mock<ITaskCommentRepository>();
            var mockHistoryRepository = new Mock<ITaskHistoryRepository>();
            mockRepository.Setup(repo => repo.GetCompletedTasksLast30DaysAsync())
                .ReturnsAsync(new List<TaskItem>
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
                });

            var mockService = new TaskItemService(mockRepository.Object, mockCommentRepository.Object, mockHistoryRepository.Object);

            // Act
            var result = await mockService.GetPerformanceReportAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.UserId == 1 && r.AverageTasksCompleted == 2);
            Assert.Contains(result, r => r.UserId == 2 && r.AverageTasksCompleted == 1);
        }

        [Fact]
        public async Task AddCommentToTask_ShouldReturnOk_WhenCommentIsAdded()
        {
            // Arrange
            var mockService = new Mock<ITaskItemService>();
            var controller = new TasksController(mockService.Object);

            var taskId = 1;
            var request = new AddCommentRequest { Comment = "Great work!", CreatedBy = "User1" };

            // Act
            var result = await controller.AddCommentToTask(taskId, request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

    }
}
