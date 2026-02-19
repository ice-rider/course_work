using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Domain.Interfaces;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<IEnumerable<Subject>> GetByTeacherIdAsync(int teacherId);
}
