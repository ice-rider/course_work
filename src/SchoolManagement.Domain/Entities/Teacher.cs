namespace SchoolManagement.Domain.Entities;

public class Teacher : AuditableEntity
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;

    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
}