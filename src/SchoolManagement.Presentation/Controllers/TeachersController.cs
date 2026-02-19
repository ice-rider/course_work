using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SchoolManagement.Application.DTOs;
using SchoolManagement.Application.UseCases;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Учителя")]
public class TeachersController : ControllerBase
{
    private readonly AddTeacherUseCase _addTeacherUseCase;
    private readonly RemoveTeacherUseCase _removeTeacherUseCase;
    private readonly ITeacherRepository _teacherRepository;

    public TeachersController(
        AddTeacherUseCase addTeacherUseCase,
        RemoveTeacherUseCase removeTeacherUseCase,
        ITeacherRepository teacherRepository)
    {
        _addTeacherUseCase = addTeacherUseCase;
        _removeTeacherUseCase = removeTeacherUseCase;
        _teacherRepository = teacherRepository;
    }

    /// <summary>
    /// Получить список всех учителей
    /// </summary>
    /// <returns>Список учителей</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Получить всех учителей")]
    [ProducesResponseType(typeof(PagedResponse<TeacherResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TeacherResponse>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 100) pageSize = 100;

        var (teachers, totalCount) = await _teacherRepository.GetPagedAsync(pageNumber, pageSize);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var response = new PagedResponse<TeacherResponse>(
            teachers.Select(t => new TeacherResponse(
                t.Id,
                t.FullName,
                t.RoomNumber,
                t.TeacherSubjects.Select(ts => ts.Subject.Name).ToList()
            )),
            pageNumber,
            pageSize,
            totalCount,
            totalPages,
            pageNumber > 1,
            pageNumber < totalPages
        );
        return Ok(response);
    }

    /// <summary>
    /// Получить учителя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор учителя</param>
    /// <returns>Данные учителя</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Получить учителя по ID")]
    [ProducesResponseType(typeof(TeacherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeacherResponse>> GetById(int id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher == null)
            return NotFound();

        return Ok(new TeacherResponse(
            teacher.Id,
            teacher.FullName,
            teacher.RoomNumber,
            teacher.TeacherSubjects.Select(ts => ts.Subject.Name).ToList()
        ));
    }

    /// <summary>
    /// Создать нового учителя
    /// </summary>
    /// <param name="request">Данные учителя</param>
    /// <returns></returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Создать учителя")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTeacherRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _addTeacherUseCase.ExecuteAsync(request.FullName, request.RoomNumber, request.Subjects);
        return Ok();
    }

    /// <summary>
    /// Удалить учителя
    /// </summary>
    /// <param name="id">Идентификатор учителя</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Удалить учителя")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _removeTeacherUseCase.ExecuteAsync(id);
        return NoContent();
    }
}