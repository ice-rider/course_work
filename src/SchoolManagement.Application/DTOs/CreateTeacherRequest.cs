using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.DTOs;

public record CreateTeacherRequest(
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
    string FullName,

    [Required(ErrorMessage = "Room number is required")]
    [StringLength(10, ErrorMessage = "Room number cannot exceed 10 characters")]
    string RoomNumber,

    [MinLength(1, ErrorMessage = "At least one subject is required")]
    List<string> Subjects
);