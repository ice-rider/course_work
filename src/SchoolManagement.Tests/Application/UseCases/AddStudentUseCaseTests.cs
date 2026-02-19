using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.UseCases;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Tests.Application.UseCases;

[TestFixture]
public class AddStudentUseCaseTests
{
    [Test]
    public async Task ExecuteAsync_ShouldAddStudentAndSaveChanges()
    {
        // Arrange - use InMemory database for integration-style test
        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new SchoolDbContext(options);
        await context.Classes.AddAsync(new Class { Name = "9А" });
        await context.SaveChangesAsync();

        var studentRepoMock = new Mock<IStudentRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        
        unitOfWorkMock.Setup(u => u.Classes).Returns(context.Classes);
        
        var useCase = new AddStudentUseCase(studentRepoMock.Object, unitOfWorkMock.Object);

        string lastName = "Иванов";
        string firstName = "Иван";
        string className = "9А";

        // Act
        await useCase.ExecuteAsync(lastName, firstName, className);

        // Assert
        studentRepoMock.Verify(r => r.AddAsync(It.Is<Student>(s =>
            s.LastName == lastName &&
            s.FirstName == firstName
        )), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void ExecuteAsync_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var studentRepoMock = new Mock<IStudentRepository>();
        studentRepoMock.Setup(r => r.AddAsync(It.IsAny<Student>()))
            .ThrowsAsync(new Exception("DB error"));

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new AddStudentUseCase(studentRepoMock.Object, unitOfWorkMock.Object);

        // Act
        Func<Task> act = async () => await useCase.ExecuteAsync("a", "b", "c");

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("DB error");
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

[TestFixture]
public class AddStudentValidationTests
{
    [TestCase("", "Иван", "9А")]
    [TestCase("Иванов", "", "9А")]
    [TestCase("Иванов", "Иван", "")]
    public async Task ExecuteAsync_ShouldThrow_WhenInputIsInvalid(string lastName, string firstName, string className)
    {
        // Arrange
        var studentRepoMock = new Mock<IStudentRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new AddStudentUseCase(studentRepoMock.Object, unitOfWorkMock.Object);

        // Act & Assert
        Func<Task> act = async () => await useCase.ExecuteAsync(lastName, firstName, className);
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
