using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.API.Application.Services;
using TaskManagement.API.Domain.Entities;
using TaskManagement.API.Domain.Interfaces;
using TaskManagement.API.Presentation.Controllers;

namespace TaskManagement.Tests.Application.Services
{
    public class ProjectServiceTests
    {
        private Mock<IProjectRepository> _mockProjectRepository;
        private IProjectService _projectService;

        public ProjectServiceTests()
        {
            _mockProjectRepository = new Mock<IProjectRepository>();
            _projectService = new ProjectService(_mockProjectRepository.Object);
        }

        [Theory]
        [MemberData(nameof(GetAllProjectsByUserIdAsync_ValidResults))]
        public async Task GetAllProjectsByUserIdAsync_ValidUserId_ReturnsProjectsAsync(List<Project> projects, int projectsCount)
        {
            // Arrange
            _mockProjectRepository.Setup(repo => repo.GetAllProjectsByUserIdAsync(It.IsAny<int>())).ReturnsAsync(projects);

            // Act
            var result = await _projectService.GetAllProjectsByUserIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(projectsCount, result.Count());
            Assert.Equal(projects, result);
        }

        public static IEnumerable<object[]> GetAllProjectsByUserIdAsync_ValidResults =>
            new List<object[]> 
            {
                new object[] 
                {
                    new List<Project> { 
                        new Project { Id = 1, Name = "Project 1", UserId = 1 },
                        new Project { Id = 2, Name = "Project 2", UserId = 1 }
                    },
                    2 
                },
                new object[]
                {
                    new List<Project> (),
                    0
                }
            };

        [Fact]
        public async Task CreateProjectAsync_ValidProject_ReturnsCreatedProjectAsync()
        {
            // Arrange
            var newProject = new Project { Name = "Project Alpha", UserId = 1 };
            var mockCreatedProject = newProject;
            mockCreatedProject.Id = 1;

            _mockProjectRepository.Setup(repo => repo.AddAsync(It.IsAny<Project>()));

            // Act
            var result = await _projectService.CreateProjectAsync(newProject);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockCreatedProject.Id, result.Id);
        }
    }
}
