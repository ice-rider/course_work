using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain;

namespace SchoolManagement.Application.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ValidGradesAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not Dictionary<string, Dictionary<string, int>> grades)
            return new ValidationResult("Grades data is invalid.");

        if (grades.Count == 0)
            return new ValidationResult("At least one student must have grades.");

        foreach (var studentEntry in grades)
        {
            if (string.IsNullOrWhiteSpace(studentEntry.Key))
                return new ValidationResult("Student name cannot be empty.");

            foreach (var subjectEntry in studentEntry.Value)
            {
                if (string.IsNullOrWhiteSpace(subjectEntry.Key))
                    return new ValidationResult("Subject name cannot be empty.");

                if (subjectEntry.Value < GradeConstants.MinValue || subjectEntry.Value > GradeConstants.MaxValue)
                    return new ValidationResult($"Grade for {studentEntry.Key}, subject {subjectEntry.Key} must be between {GradeConstants.MinValue} and {GradeConstants.MaxValue}.");
            }
        }

        return ValidationResult.Success;
    }
}