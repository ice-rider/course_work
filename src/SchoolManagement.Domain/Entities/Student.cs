using SchoolManagement.Domain.Entities;

public class Student : AuditableEntity
{
    public int Id { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;

    public int ClassId { get; set; }          
    public Class? Class { get; set; }

    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}