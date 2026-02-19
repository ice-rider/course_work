using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Infrastructure.Repositories;

public class SubjectRepository : EfRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(SchoolDbContext context) : base(context) { }

    public async Task<IEnumerable<Subject>> GetByTeacherIdAsync(int teacherId)
    {
        return await _context.Subjects
            .Include(s => s.TeacherSubjects)
            .Where(s => s.TeacherSubjects.Any(ts => ts.TeacherId == teacherId))
            .ToListAsync();
    }
}