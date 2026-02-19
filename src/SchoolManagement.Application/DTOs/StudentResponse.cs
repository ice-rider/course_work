using SchoolManagement.Domain.Entities;

public record StudentResponse(
    int Id,
    string LastName,
    string FirstName,
    string? ClassName
);