using Moq;
using NUnit.Framework;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.UseCases;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Tests.Application.UseCases;

[TestFixture]
public class UpdateGradeUseCaseTests
{
    [Test]
    public async Task ExecuteAsync_WithValidData_ShouldUpdateGrade()
    {
        // Arrange
        var grade = new Grade
        {
            Id = 1,
            StudentId = 1,
            SubjectId = 1,
            Value = 3,
            Quarter = 1,
            Year = 2025
        };

        var student = new Student
        {
            Id = 1,
            LastName = "Иванов",
            FirstName = "Иван",
            ClassId = 1
        };

        var subject = new Subject { Id = 1, Name = "Математика" };

        var gradeRepoMock = new Mock<IGradeRepository>();
        var studentRepoMock = new Mock<IStudentRepository>();
        var subjectRepoMock = new Mock<ISubjectRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        gradeRepoMock.Setup(r => r.GetGradeWithDetailsAsync(1)).ReturnsAsync(grade);
        studentRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Student> { student });
        subjectRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Subject> { subject });

        var useCase = new UpdateGradeUseCase(
            gradeRepoMock.Object,
            studentRepoMock.Object,
            subjectRepoMock.Object,
            unitOfWorkMock.Object);

        // Act
        await useCase.ExecuteAsync(1, "Иванов Иван", "Математика", 5);

        // Assert
        Assert.That(grade.Value, Is.EqualTo(5));
        Assert.That(grade.StudentId, Is.EqualTo(1));
        Assert.That(grade.SubjectId, Is.EqualTo(1));
        gradeRepoMock.Verify(r => r.Update(grade), Times.Once);
        unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidGradeValue_ShouldThrowArgumentException()
    {
        // Arrange
        var grade = new Grade { Id = 1, StudentId = 1, SubjectId = 1, Value = 3 };

        var gradeRepoMock = new Mock<IGradeRepository>();
        var studentRepoMock = new Mock<IStudentRepository>();
        var subjectRepoMock = new Mock<ISubjectRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        gradeRepoMock.Setup(r => r.GetGradeWithDetailsAsync(1)).ReturnsAsync(grade);

        var useCase = new UpdateGradeUseCase(
            gradeRepoMock.Object,
            studentRepoMock.Object,
            subjectRepoMock.Object,
            unitOfWorkMock.Object);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(
            () => useCase.ExecuteAsync(1, "Иванов Иван", "Математика", 6));
        
        Assert.That(exception!.Message, Does.Contain("Grade must be between 2 and 5"));
    }

    [Test]
    public void ExecuteAsync_WithNonExistentGrade_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var gradeRepoMock = new Mock<IGradeRepository>();
        var studentRepoMock = new Mock<IStudentRepository>();
        var subjectRepoMock = new Mock<ISubjectRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        gradeRepoMock.Setup(r => r.GetGradeWithDetailsAsync(999)).ReturnsAsync((Grade?)null);

        var useCase = new UpdateGradeUseCase(
            gradeRepoMock.Object,
            studentRepoMock.Object,
            subjectRepoMock.Object,
            unitOfWorkMock.Object);

        // Act & Assert
        var exception = Assert.ThrowsAsync<KeyNotFoundException>(
            () => useCase.ExecuteAsync(999, "Иванов Иван", "Математика", 5));

        Assert.That(exception!.Message, Does.Contain("Grade with ID 999 not found"));
    }

    [Test]
    public void ExecuteAsync_WithNonExistentStudent_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var grade = new Grade { Id = 1, StudentId = 1, SubjectId = 1, Value = 3 };

        var gradeRepoMock = new Mock<IGradeRepository>();
        var studentRepoMock = new Mock<IStudentRepository>();
        var subjectRepoMock = new Mock<ISubjectRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        gradeRepoMock.Setup(r => r.GetGradeWithDetailsAsync(1)).ReturnsAsync(grade);
        studentRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Student>());

        var useCase = new UpdateGradeUseCase(
            gradeRepoMock.Object,
            studentRepoMock.Object,
            subjectRepoMock.Object,
            unitOfWorkMock.Object);

        // Act & Assert
        var exception = Assert.ThrowsAsync<KeyNotFoundException>(
            () => useCase.ExecuteAsync(1, "Иванов Иван", "Математика", 5));

        Assert.That(exception!.Message, Does.Contain("Student 'Иванов Иван' not found"));
    }

    [Test]
    public void ExecuteAsync_WithNonExistentSubject_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var grade = new Grade { Id = 1, StudentId = 1, SubjectId = 1, Value = 3 };
        var student = new Student { Id = 1, LastName = "Иванов", FirstName = "Иван", ClassId = 1 };

        var gradeRepoMock = new Mock<IGradeRepository>();
        var studentRepoMock = new Mock<IStudentRepository>();
        var subjectRepoMock = new Mock<ISubjectRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        gradeRepoMock.Setup(r => r.GetGradeWithDetailsAsync(1)).ReturnsAsync(grade);
        studentRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Student> { student });
        subjectRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Subject>());

        var useCase = new UpdateGradeUseCase(
            gradeRepoMock.Object,
            studentRepoMock.Object,
            subjectRepoMock.Object,
            unitOfWorkMock.Object);

        // Act & Assert
        var exception = Assert.ThrowsAsync<KeyNotFoundException>(
            () => useCase.ExecuteAsync(1, "Иванов Иван", "Математика", 5));

        Assert.That(exception!.Message, Does.Contain("Subject 'Математика' not found"));
    }

    [Test]
    public async Task ExecuteAsync_WithDifferentStudentName_ShouldUpdateStudentId()
    {
        // Arrange
        var grade = new Grade { Id = 1, StudentId = 1, SubjectId = 1, Value = 4 };

        var student1 = new Student { Id = 1, LastName = "Иванов", FirstName = "Иван", ClassId = 1 };
        var student2 = new Student { Id = 2, LastName = "Петров", FirstName = "Петр", ClassId = 1 };
        var subject = new Subject { Id = 1, Name = "Математика" };

        var gradeRepoMock = new Mock<IGradeRepository>();
        var studentRepoMock = new Mock<IStudentRepository>();
        var subjectRepoMock = new Mock<ISubjectRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        gradeRepoMock.Setup(r => r.GetGradeWithDetailsAsync(1)).ReturnsAsync(grade);
        studentRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Student> { student1, student2 });
        subjectRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Subject> { subject });

        var useCase = new UpdateGradeUseCase(
            gradeRepoMock.Object,
            studentRepoMock.Object,
            subjectRepoMock.Object,
            unitOfWorkMock.Object);

        // Act
        await useCase.ExecuteAsync(1, "Петров Петр", "Математика", 5);

        // Assert
        Assert.That(grade.StudentId, Is.EqualTo(2));
        Assert.That(grade.Value, Is.EqualTo(5));
    }
}
