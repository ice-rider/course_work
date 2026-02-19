using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Domain.Interfaces;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<IEnumerable<Teacher>> GetBySubjectAsync(string subjectName); // пример
}