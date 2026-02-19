using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Domain.Interfaces;

public interface IGradeRepository : IRepository<Grade>
{
    Task<IEnumerable<Grade>> GetGradesForClassQuarterAsync(string className, int quarter);
    Task AddRangeAsync(IEnumerable<Grade> grades); // массовое добавление оценок
    Task<Grade?> GetGradeWithDetailsAsync(int id); // получение оценки с деталями
}
