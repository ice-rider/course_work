using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SchoolManagement.Application.DTOs;
using SchoolManagement.Application.UseCases;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Ученики")]
public class StudentsController : ControllerBase
{
    private readonly AddStudentUseCase _addStudentUseCase;
    private readonly RemoveStudentUseCase _removeStudentUseCase;
    private readonly IStudentRepository _studentRepository;

    public StudentsController(
        AddStudentUseCase addStudentUseCase,
        RemoveStudentUseCase removeStudentUseCase,
        IStudentRepository studentRepository)
    {
        _addStudentUseCase = addStudentUseCase;
        _removeStudentUseCase = removeStudentUseCase;
        _studentRepository = studentRepository;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Получить всех учеников")]
    [ProducesResponseType(typeof(PagedResponse<StudentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<StudentResponse>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 100) pageSize = 100;

        var (students, totalCount) = await _studentRepository.GetPagedAsync(pageNumber, pageSize);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var response = new PagedResponse<StudentResponse>(
            students.Select(s => new StudentResponse(s.Id, s.LastName, s.FirstName, s.Class?.Name)),
            pageNumber,
            pageSize,
            totalCount,
            totalPages,
            pageNumber > 1,
            pageNumber < totalPages
        );
        return Ok(response);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Получить ученика по ID")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentResponse>> GetById(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null)
            return NotFound();

        return Ok(new StudentResponse(
            student.Id,
            student.LastName,
            student.FirstName,
            student.Class?.Name
        ));
    }

    [HttpGet("by-class/{className}")]
    [SwaggerOperation(Summary = "Получить учеников по названию класса")]
    [ProducesResponseType(typeof(IEnumerable<StudentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StudentResponse>>> GetByClass(string className)
    {
        var students = await _studentRepository.GetByClassAsync(className);
        var response = students.Select(s => new StudentResponse(
            s.Id,
            s.LastName,
            s.FirstName,
            s.Class?.Name
        ));
        return Ok(response);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Создать нового ученика")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _addStudentUseCase.ExecuteAsync(request.LastName, request.FirstName, request.ClassName);
        return Ok();
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Удалить ученика")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _removeStudentUseCase.ExecuteAsync(id);
        return NoContent();
    }
}