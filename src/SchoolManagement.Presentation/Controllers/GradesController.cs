using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SchoolManagement.Application.DTOs;
using SchoolManagement.Application.UseCases;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Оценки")]
public class GradesController : ControllerBase
{
    private readonly AddGradesUseCase _addGradesUseCase;
    private readonly UpdateGradeUseCase _updateGradeUseCase;
    private readonly IGradeRepository _gradeRepository;

    public GradesController(
        AddGradesUseCase addGradesUseCase,
        UpdateGradeUseCase updateGradeUseCase,
        IGradeRepository gradeRepository)
    {
        _addGradesUseCase = addGradesUseCase;
        _updateGradeUseCase = updateGradeUseCase;
        _gradeRepository = gradeRepository;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Добавить оценки для класса за четверть")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddGrades([FromBody] AddGradesRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _addGradesUseCase.ExecuteAsync(request.ClassName, request.Quarter, request.Year, request.GradesByStudent);
        return Ok();
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Обновить оценку")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGradeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _updateGradeUseCase.ExecuteAsync(id, request.StudentName, request.SubjectName, request.Value);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("class/{className}/quarter/{quarter}")]
    [SwaggerOperation(Summary = "Получить оценки класса за четверть")]
    [ProducesResponseType(typeof(PagedResponse<GradeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<GradeResponse>>> GetGradesForClassQuarter(
        string className,
        int quarter,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 200) pageSize = 200;

        var allGrades = await _gradeRepository.GetGradesForClassQuarterAsync(className, quarter);
        var totalCount = allGrades.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var grades = allGrades.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        var response = new PagedResponse<GradeResponse>(
            grades.Select(g => new GradeResponse(
                g.Id,
                g.StudentId,
                $"{g.Student?.LastName} {g.Student?.FirstName}",
                g.SubjectId,
                g.Subject?.Name ?? string.Empty,
                g.Value,
                g.Quarter,
                g.Year
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
}