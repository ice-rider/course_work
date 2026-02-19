using Moq;
using NUnit.Framework;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.UseCases;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Tests.Application.UseCases;

[TestFixture]
public class RemoveStudentUseCaseTests
{
    [Test]
    public async Task ExecuteAsync_WhenStudentExists_ShouldDeleteAndSaveChanges()
    {
        // Arrange
        var studentRepoMock = new Mock<IStudentRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var student = new Student { Id = 1, LastName = "Ivanov", FirstName = "Ivan" };
        studentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(student);

        var useCase = new RemoveStudentUseCase(studentRepoMock.Object, unitOfWorkMock.Object);

        // Act
        await useCase.ExecuteAsync(1);

        // Assert
        studentRepoMock.Verify(r => r.Delete(student), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WhenStudentDoesNotExist_ShouldNotDelete()
    {
        // Arrange
        var studentRepoMock = new Mock<IStudentRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        studentRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Student?)null);

        var useCase = new RemoveStudentUseCase(studentRepoMock.Object, unitOfWorkMock.Object);

        // Act
        await useCase.ExecuteAsync(999);

        // Assert
        studentRepoMock.Verify(r => r.Delete(It.IsAny<Student>()), Times.Never);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
