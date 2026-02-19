using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Repositories;

namespace SchoolManagement.Tests.Application.UseCases;

[TestFixture]
public class StudentRepositoryTests
{
    private SchoolDbContext? _context;
    private StudentRepository? _repository;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new SchoolDbContext(options);
        _repository = new StudentRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context!.Dispose();
    }

    [Test]
    public async Task GetByClassAsync_ShouldReturnOnlyStudentsFromGivenClass()
    {
        // Arrange
        var class9A = new Class { Name = "9A" };
        var class8B = new Class { Name = "8B" };
        
        _context!.Classes.AddRange(class9A, class8B);
        _context.Students.AddRange(
            new Student { LastName = "Ivanov", FirstName = "Ivan", ClassId = class9A.Id },
            new Student { LastName = "Petrov", FirstName = "Petr", ClassId = class9A.Id },
            new Student { LastName = "Sidorov", FirstName = "Sidor", ClassId = class8B.Id }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository!.GetByClassAsync("9A");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.LastName == "Ivanov");
        result.Should().Contain(s => s.LastName == "Petrov");
    }
}