using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.UseCases;
using SchoolManagement.Domain.Interfaces;
using SchoolManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.All;
    o.RequestBodyLogLimit = 4096;
    o.ResponseBodyLogLimit = 4096;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options
        .UseSqlite(connectionString, b => b.MigrationsAssembly("course_work"))
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
);

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<AddTeacherUseCase>();
builder.Services.AddScoped<AddStudentUseCase>();
builder.Services.AddScoped<AddGradesUseCase>();
builder.Services.AddScoped<UpdateGradeUseCase>();
builder.Services.AddScoped<RemoveTeacherUseCase>();
builder.Services.AddScoped<RemoveStudentUseCase>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpLogging();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
