using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.DTOs;

public record CreateStudentRequest(
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
    string LastName,

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
    string FirstName,

    [Required(ErrorMessage = "Class name is required")]
    [StringLength(10, ErrorMessage = "Class name cannot exceed 10 characters")]
    string ClassName
);