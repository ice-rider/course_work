using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Infrastructure.Repositories;

public class TeacherRepository : EfRepository<Teacher>, ITeacherRepository
{
    public TeacherRepository(SchoolDbContext context) : base(context) { }

    public async Task<IEnumerable<Teacher>> GetBySubjectAsync(string subjectName)
    {
        return await _context.Teachers
            .Include(t => t.TeacherSubjects)
            .ThenInclude(ts => ts.Subject)
            .Where(t => t.TeacherSubjects.Any(ts => ts.Subject.Name == subjectName))
            .ToListAsync();
    }
}