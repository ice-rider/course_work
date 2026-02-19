namespace SchoolManagement.Domain.Entities;

// in this project class with name 'Class' mean class in school
public class Class : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}