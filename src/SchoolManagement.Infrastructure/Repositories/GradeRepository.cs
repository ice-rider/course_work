using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagement.Infrastructure.Repositories;

public class GradeRepository : EfRepository<Grade>, IGradeRepository
{
    public GradeRepository(SchoolDbContext context) : base(context) { }

    public async Task<IEnumerable<Grade>> GetGradesForClassQuarterAsync(string className, int quarter)
    {
        return await _context.Grades
            .Include(g => g.Student)
            .ThenInclude(s => s!.Class)
            .Include(g => g.Subject)
            .Where(g => g.Student!.Class != null && g.Student.Class.Name == className && g.Quarter == quarter)
            .ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Grade> grades)
    {
        await _context.Grades.AddRangeAsync(grades);
    }

    public async Task<Grade?> GetGradeWithDetailsAsync(int id)
    {
        return await _context.Grades
            .Include(g => g.Student)
            .Include(g => g.Subject)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}