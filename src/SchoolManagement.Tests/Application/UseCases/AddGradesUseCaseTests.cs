using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.UseCases;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Tests.Application.UseCases;

[TestFixture]
public class AddGradesUseCaseTests
{
    [Test]
    public async Task ExecuteAsync_ShouldAddGradesOnlyForMatchingStudentsAndExistingSubjects()
    {
        // Arrange - use InMemory database for integration-style test
        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new SchoolDbContext(options);
        
        await context.Classes.AddAsync(new Class { Name = "9А" });
        await context.Subjects.AddRangeAsync(
            new Subject { Name = "Математика" },
            new Subject { Name = "Физика" }
        );
        await context.SaveChangesAsync();

        var students = new List<Student>
        {
            new Student { Id = 1, LastName = "Иванов", FirstName = "Иван", ClassId = 1 },
            new Student { Id = 2, LastName = "Петров", FirstName = "Петр", ClassId = 1 }
        };

        var gradeRepoMock = new Mock<IGradeRepository>();
        var studentRepoMock = new Mock<IStudentRepository>();
        var subjectRepoMock = new Mock<ISubjectRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        
        studentRepoMock.Setup(r => r.GetByClassAsync("9А")).ReturnsAsync(students);
        subjectRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(context.Subjects.ToList());
        unitOfWorkMock.Setup(u => u.Classes).Returns(context.Classes);

        var gradesByStudent = new Dictionary<string, Dictionary<string, int>>
        {
            ["Иванов Иван"] = new() { ["Математика"] = 5, ["Физика"] = 4, ["Химия"] = 3 }, // Химия не существует
            ["Петров Петр"] = new() { ["Математика"] = 4, ["Физика"] = 5 },
            ["Сидоров Сидор"] = new() { ["Математика"] = 3 } // этого ученика нет в классе
        };

        var useCase = new AddGradesUseCase(
            gradeRepoMock.Object,
            studentRepoMock.Object,
            subjectRepoMock.Object,
            unitOfWorkMock.Object);

        // Act
        await useCase.ExecuteAsync("9А", 1, 2026, gradesByStudent);

        // Assert
        // Проверяем, что AddRangeAsync вызван с 4 оценками (2 ученика * 2 предмета)
        gradeRepoMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Grade>>(grades =>
            grades.Count() == 4 &&
            grades.Any(g => g.StudentId == 1 && g.SubjectId == 1 && g.Value == 5) &&
            grades.Any(g => g.StudentId == 1 && g.SubjectId == 2 && g.Value == 4) &&
            grades.Any(g => g.StudentId == 2 && g.SubjectId == 1 && g.Value == 4) &&
            grades.Any(g => g.StudentId == 2 && g.SubjectId == 2 && g.Value == 5)
        )), Times.Once);

        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
