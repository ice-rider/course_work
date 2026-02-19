using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain;
using SchoolManagement.Domain.Entities;

public class SchoolDbContext : DbContext
{
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<TeacherSubject> TeacherSubjects { get; set; }

    public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Class)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Grade>()
            .HasOne(g => g.Student)
            .WithMany(s => s.Grades)
            .HasForeignKey(g => g.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Grade>()
            .HasOne(g => g.Subject)
            .WithMany()
            .HasForeignKey(g => g.SubjectId);

        modelBuilder.Entity<TeacherSubject>()
            .HasKey(ts => new { ts.TeacherId, ts.SubjectId });

        modelBuilder.Entity<TeacherSubject>()
            .HasOne(ts => ts.Teacher)
            .WithMany(t => t.TeacherSubjects)
            .HasForeignKey(ts => ts.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeacherSubject>()
            .HasOne(ts => ts.Subject)
            .WithMany(s => s.TeacherSubjects)
            .HasForeignKey(ts => ts.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Teacher>()
            .HasIndex(t => t.RoomNumber);

        modelBuilder.Entity<Subject>()
            .HasIndex(s => s.Name)
            .IsUnique();

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.ClassId);

        modelBuilder.Entity<Grade>()
            .HasIndex(g => new { g.StudentId, g.Year, g.Quarter });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Grades_Value", $"\"Value\" BETWEEN {GradeConstants.MinValue} AND {GradeConstants.MaxValue}");
                t.HasCheckConstraint("CK_Grades_Quarter", $"\"Quarter\" BETWEEN {GradeConstants.MinQuarter} AND {GradeConstants.MaxQuarter}");
            });
        });
    }
}