using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Application.UseCases;

public class AddGradesUseCase
{
    private readonly IGradeRepository _gradeRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddGradesUseCase(IGradeRepository gradeRepository,
                           IStudentRepository studentRepository,
                           ISubjectRepository subjectRepository,
                           IUnitOfWork unitOfWork)
    {
        _gradeRepository = gradeRepository;
        _studentRepository = studentRepository;
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(string className, int quarter, int year, Dictionary<string, Dictionary<string, int>> gradesByStudent)
    {
        var @class = await _unitOfWork.Classes.FirstOrDefaultAsync(c => c.Name == className);
        if (@class == null)
            throw new ArgumentException($"Class {className} not found", nameof(className));

        var students = (await _studentRepository.GetByClassAsync(className)).ToList();
        var grades = new List<Grade>();
        var processedGrades = new HashSet<(int StudentId, int SubjectId)>();

        foreach (var student in students)
        {
            var fullName = $"{student.LastName} {student.FirstName}";
            if (gradesByStudent.TryGetValue(fullName, out var subjectGrades))
            {
                foreach (var (subjectName, gradeValue) in subjectGrades)
                {
                    var subject = await GetSubjectByNameAsync(subjectName);
                    if (subject != null)
                    {
                        if (!processedGrades.Add((student.Id, subject.Id)))
                            throw new InvalidOperationException($"Duplicate grade for student {fullName} and subject {subjectName}");

                        grades.Add(new Grade
                        {
                            StudentId = student.Id,
                            SubjectId = subject.Id,
                            Value = gradeValue,
                            Quarter = quarter,
                            Year = year
                        });
                    }
                }
            }
        }
        await _gradeRepository.AddRangeAsync(grades);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Subject?> GetSubjectByNameAsync(string name)
    {
        var subjects = await _subjectRepository.GetAllAsync();
        return subjects.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
