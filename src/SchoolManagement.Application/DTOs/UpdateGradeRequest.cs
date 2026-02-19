using System.ComponentModel.DataAnnotations;
using SchoolManagement.Application.Attributes;

namespace SchoolManagement.Application.DTOs;

public record UpdateGradeRequest(
    [Required(ErrorMessage = "Student name is required")]
    string StudentName,

    [Required(ErrorMessage = "Subject name is required")]
    string SubjectName,

    [Required(ErrorMessage = "Grade value is required")]
    [System.ComponentModel.DataAnnotations.Range(2, 5, ErrorMessage = "Grade must be between 2 and 5")]
    int Value
);
