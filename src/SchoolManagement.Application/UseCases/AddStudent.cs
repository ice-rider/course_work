using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Application.UseCases;

public class AddStudentUseCase
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddStudentUseCase(IStudentRepository studentRepository, IUnitOfWork unitOfWork) {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(string lastName, string firstName, string className)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(className))
            throw new ArgumentException("Class name cannot be empty", nameof(className));

        var @class = await _unitOfWork.Classes.FirstOrDefaultAsync(c => c.Name == className);
        if (@class == null)
        {
            @class = new Class { Name = className };
            await _unitOfWork.Classes.AddAsync(@class);
            await _unitOfWork.SaveChangesAsync();
        }

        var student = new Student { LastName = lastName, FirstName = firstName, ClassId = @class.Id };
        await _studentRepository.AddAsync(student);
        await _unitOfWork.SaveChangesAsync();
    }
}