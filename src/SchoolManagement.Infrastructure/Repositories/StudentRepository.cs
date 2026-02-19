using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Infrastructure.Repositories;

public class StudentRepository : EfRepository<Student>, IStudentRepository
{
    public StudentRepository(SchoolDbContext context) : base(context) { }

    public async Task<IEnumerable<Student>> GetByClassAsync(string className)
    {
        return await _context.Students
            .Include(s => s.Class)
            .Where(s => s.Class != null && s.Class.Name == className)
            .ToListAsync();
    }
}