using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Interfaces;

public interface IUnitOfWork
{
    DbSet<Teacher> Teachers { get; }
    DbSet<Student> Students { get; }
    DbSet<Subject> Subjects { get; }
    DbSet<Grade> Grades { get; }
    DbSet<TeacherSubject> TeacherSubjects { get; }
    DbSet<Class> Classes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}