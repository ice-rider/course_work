public record GradeResponse(
    int Id,
    int StudentId,
    string StudentName,
    int SubjectId,
    string SubjectName,
    int Value,
    int Quarter,
    int Year
);