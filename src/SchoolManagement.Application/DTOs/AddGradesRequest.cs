using System.ComponentModel.DataAnnotations;
using SchoolManagement.Application.Attributes;

namespace SchoolManagement.Application.DTOs;

public record AddGradesRequest
{
    [Required(ErrorMessage = "Class name is required")]
    public string ClassName { get; init; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Range(1, 4, ErrorMessage = "Quarter must be between 1 and 4")]
    public int Quarter { get; init; }

    [Required(ErrorMessage = "Year is required")]
    public int Year { get; init; }

    [Required(ErrorMessage = "Grades data is required")]
    [ValidGrades]
    public Dictionary<string, Dictionary<string, int>> GradesByStudent { get; init; } = new();
}