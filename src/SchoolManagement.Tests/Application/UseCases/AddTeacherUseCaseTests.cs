using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.UseCases;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Tests.Application.UseCases;

[TestFixture]
public class AddTeacherUseCaseTests
{
    [Test]
    public async Task ExecuteAsync_ShouldAddTeacherWithSubjects()
    {
        // Arrange - use InMemory database for integration-style test
        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new SchoolDbContext(options);
        
        // Pre-populate subjects
        await context.Subjects.AddRangeAsync(
            new Subject { Name = "Математика" },
            new Subject { Name = "Физика" }
        );
        await context.SaveChangesAsync();

        var teacherRepoMock = new Mock<ITeacherRepository>();
        var subjectRepoMock = new Mock<ISubjectRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        
        // Set up mock to return subjects from context
        subjectRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(() => context.Subjects.ToList());

        unitOfWorkMock.Setup(u => u.Subjects).Returns(context.Subjects);
        unitOfWorkMock.Setup(u => u.TeacherSubjects).Returns(context.Set<TeacherSubject>());

        var useCase = new AddTeacherUseCase(teacherRepoMock.Object, subjectRepoMock.Object, unitOfWorkMock.Object);

        string fullName = "Петров Пётр";
        string roomNumber = "101";
        var subjects = new List<string> { "Математика", "Физика" };

        // Act
        await useCase.ExecuteAsync(fullName, roomNumber, subjects);

        // Assert
        teacherRepoMock.Verify(r => r.AddAsync(It.Is<Teacher>(t =>
            t.FullName == fullName &&
            t.RoomNumber == roomNumber
        )), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}
