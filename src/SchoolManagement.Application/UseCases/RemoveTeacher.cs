using SchoolManagement.Domain.Interfaces;
using SchoolManagement.Application.Interfaces;

namespace SchoolManagement.Application.UseCases;

public class RemoveTeacherUseCase
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RemoveTeacherUseCase(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork) {
        _teacherRepository = teacherRepository;
        _unitOfWork = unitOfWork;
    } 

    public async Task ExecuteAsync(int teacherId)
    {
        var teacher = await _teacherRepository.GetByIdAsync(teacherId);
        if (teacher != null)
        {
            _teacherRepository.Delete(teacher);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}