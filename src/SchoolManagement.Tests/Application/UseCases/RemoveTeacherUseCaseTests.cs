using Moq;
using NUnit.Framework;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.UseCases;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Tests.Application.UseCases;

[TestFixture]
public class RemoveTeacherUseCaseTests
{
    [Test]
    public async Task ExecuteAsync_WhenTeacherExists_ShouldDeleteAndSaveChanges()
    {
        // Arrange
        var teacherRepoMock = new Mock<ITeacherRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var teacher = new Teacher { Id = 1, FullName = "Test Teacher", RoomNumber = "101" };
        teacherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(teacher);

        var useCase = new RemoveTeacherUseCase(teacherRepoMock.Object, unitOfWorkMock.Object);

        // Act
        await useCase.ExecuteAsync(1);

        // Assert
        teacherRepoMock.Verify(r => r.Delete(teacher), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WhenTeacherDoesNotExist_ShouldNotDelete()
    {
        // Arrange
        var teacherRepoMock = new Mock<ITeacherRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        teacherRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Teacher?)null);

        var useCase = new RemoveTeacherUseCase(teacherRepoMock.Object, unitOfWorkMock.Object);

        // Act
        await useCase.ExecuteAsync(999);

        // Assert
        teacherRepoMock.Verify(r => r.Delete(It.IsAny<Teacher>()), Times.Never);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
