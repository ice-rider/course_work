using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Application.UseCases;

public class UpdateGradeUseCase
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGradeUseCase(
        IGradeRepository gradeRepository,
        IStudentRepository studentRepository,
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork)
    {
        _gradeRepository = gradeRepository;
        _studentRepository = studentRepository;
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int gradeId,
        string studentName,
        string subjectName,
        int value)
    {
        // Валидация значения оценки
        if (value < GradeConstants.MinValue || value > GradeConstants.MaxValue)
            throw new ArgumentException($"Grade must be between {GradeConstants.MinValue} and {GradeConstants.MaxValue}", nameof(value));

        // Получение существующей оценки
        var grade = await _gradeRepository.GetGradeWithDetailsAsync(gradeId);
        if (grade == null)
            throw new KeyNotFoundException($"Grade with ID {gradeId} not found");

        // Поиск ученика по имени
        var student = await FindStudentByNameAsync(studentName);
        if (student == null)
            throw new KeyNotFoundException($"Student '{studentName}' not found");

        // Поиск предмета по названию
        var subject = await GetSubjectByNameAsync(subjectName);
        if (subject == null)
            throw new KeyNotFoundException($"Subject '{subjectName}' not found");

        // Обновление оценки
        grade.StudentId = student.Id;
        grade.SubjectId = subject.Id;
        grade.Value = value;
        grade.UpdatedAt = DateTime.UtcNow;

        _gradeRepository.Update(grade);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Student?> FindStudentByNameAsync(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
            return null;

        var lastName = parts[0];
        var firstName = parts[1];

        var students = await _studentRepository.GetAllAsync();
        return students.FirstOrDefault(s =>
            s.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase) &&
            s.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<Subject?> GetSubjectByNameAsync(string name)
    {
        var subjects = await _subjectRepository.GetAllAsync();
        return subjects.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
