using SchoolManagement.Domain.Interfaces;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.UseCases;

public class RemoveStudentUseCase
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveStudentUseCase(IStudentRepository studentRepository, IUnitOfWork unitOfWork) {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student != null)
        {
            _studentRepository.Delete(student);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}