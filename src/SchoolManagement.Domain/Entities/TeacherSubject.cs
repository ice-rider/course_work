using SchoolManagement.Domain.Entities;

public class TeacherSubject : AuditableEntity
{
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
}