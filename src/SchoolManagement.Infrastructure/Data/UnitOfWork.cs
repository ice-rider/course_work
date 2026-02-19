using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;

public class UnitOfWork : IUnitOfWork
{
    private readonly SchoolDbContext _context;

    public UnitOfWork(SchoolDbContext context)
    {
        _context = context;
    }

    public DbSet<Teacher> Teachers => _context.Teachers;
    public DbSet<Student> Students => _context.Students;
    public DbSet<Subject> Subjects => _context.Subjects;
    public DbSet<Grade> Grades => _context.Grades;
    public DbSet<TeacherSubject> TeacherSubjects => _context.Set<TeacherSubject>();
    public DbSet<Class> Classes => _context.Set<Class>();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}