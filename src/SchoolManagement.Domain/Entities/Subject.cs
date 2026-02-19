namespace SchoolManagement.Domain.Entities;

public class Subject : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
}