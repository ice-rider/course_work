namespace SchoolManagement.Domain.Entities;

public class Grade : AuditableEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int Value { get; set; }
    public int Year { get; set; }
    public int Quarter { get; set; }

    public Student? Student { get; set; }
    public Subject? Subject { get; set; }
}