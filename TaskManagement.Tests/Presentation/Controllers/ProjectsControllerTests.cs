using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Interfaces;
using TaskManagement.API.Presentation.Controllers;

namespace TaskManagement.Tests.Presentation.Controllers
{
    public class ProjectsControllerTests
    {
        [Fact]
        public async Task GetAllProjectsByUserIdAsync_ShouldReturnListOfProjectsAsync()
        {
            // Arrange
            var mockService = new Mock<IProjectService>();
            var userId = 1;
            var mockProjects = new List<Project>
            {
                new Project { Id = 1, Name = "Project Alpha", UserId = userId },
                new Project { Id = 2, Name = "Project Beta", UserId = userId }
            };
            mockService.Setup(service => service.GetAllProjectsByUserIdAsync(It.IsAny<int>())).ReturnsAsync(mockProjects);

            var controller = new ProjectsController(mockService.Object);

            // Act
            var result = await controller.GetAllProjectsByUserIdAsync(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProjects = Assert.IsAssignableFrom<IEnumerable<Project>>(okResult.Value);
            Assert.NotNull(returnedProjects);
            Assert.Equal(2, returnedProjects.Count());
            Assert.Contains(returnedProjects, p => p.Name == "Project Alpha");
        }

        [Fact]
        public async Task CreateProjectAsync_ShouldReturnCreatedProjectAsync()
        {
            // Arrange
            var mockService = new Mock<IProjectService>();
            var newProject = new Project { Name = "Project Alpha", UserId = 1 };
            var mockCreatedProject = newProject;
            mockCreatedProject.Id = 1;
            mockService.Setup(service => service.CreateProjectAsync(It.IsAny<Project>())).ReturnsAsync(newProject);

            var controller = new ProjectsController(mockService.Object);

            // Act
            var result = await controller.CreateProjectAsync(newProject);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedProject = Assert.IsAssignableFrom<Project>(createdResult.Value);
            Assert.NotNull(returnedProject);
            Assert.Equal(mockCreatedProject.Id, returnedProject.Id);
        }
    }
}
