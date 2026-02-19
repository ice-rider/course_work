using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.UseCases;

public class AddTeacherUseCase
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddTeacherUseCase(ITeacherRepository teacherRepository, ISubjectRepository subjectRepository, IUnitOfWork unitOfWork)
    {
        _teacherRepository = teacherRepository;
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(string fullName, string roomNumber, IEnumerable<string> subjects)
    {
        var teacher = new Teacher
        {
            FullName = fullName,
            RoomNumber = roomNumber
        };

        await _teacherRepository.AddAsync(teacher);
        await _unitOfWork.SaveChangesAsync();

        var allSubjects = (await _subjectRepository.GetAllAsync()).ToList();
        var existingTeacherSubjectIds = new HashSet<(int TeacherId, int SubjectId)>();

        foreach (var subjectName in subjects)
        {
            var subject = allSubjects.FirstOrDefault(s => s.Name.Equals(subjectName, StringComparison.OrdinalIgnoreCase));
            if (subject == null)
            {
                subject = new Subject { Name = subjectName };
                await _subjectRepository.AddAsync(subject);
                await _unitOfWork.SaveChangesAsync();
                allSubjects.Add(subject);
            }

            // Check for duplicates
            if (existingTeacherSubjectIds.Contains((teacher.Id, subject.Id)))
                throw new InvalidOperationException($"Subject '{subjectName}' is already assigned to this teacher");

            var teacherSubject = new TeacherSubject
            {
                TeacherId = teacher.Id,
                SubjectId = subject.Id
            };
            await _unitOfWork.TeacherSubjects.AddAsync(teacherSubject);
            existingTeacherSubjectIds.Add((teacher.Id, subject.Id));
        }
        await _unitOfWork.SaveChangesAsync();
    }
}