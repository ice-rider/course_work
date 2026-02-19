# School Management System

A comprehensive ASP.NET Core Web API for school management, designed to automate administrative tasks for school coordinators. This system handles teacher management, student enrollment, class assignments, subject tracking, and grade recording.

## 📋 Features

### Core Functionality

- **Teacher Management**
  - Register and manage teacher profiles
  - Assign multiple subjects to teachers
  - Assign a single classroom to each teacher

- **Student Management**
  - Enroll new students
  - Assign students to classes
  - Transfer students between classes

- **Class Management**
  - Create and manage school classes (e.g., "5Б", "10А")
  - Independent class entities that exist regardless of student enrollment

- **Subject Management**
  - Dynamic subject catalog (not fixed)
  - Create and manage academic subjects

- **Grade Management**
  - Record grades by class (bulk entry for all students in a class)
  - Record grades by student (individual entry)
  - Grade validation (2-5 scale only)
  - Immutable grades after submission

## 🏗️ Architecture

The project follows a clean architecture pattern with clear separation of concerns:

```
src/
├── SchoolManagement.Application/    # Business logic and use cases
├── SchoolManagement.Domain/         # Core entities and interfaces
├── SchoolManagement.Infrastructure/ # Data access and repositories
├── SchoolManagement.Presentation/   # API controllers
├── SchoolManagement.Middleware/     # Custom middleware components
└── SchoolManagement.Tests/          # Unit and integration tests
```

## 🛠️ Tech Stack

- **Framework**: .NET 10.0
- **Database**: SQLite with Entity Framework Core
- **API Documentation**: Swagger/OpenAPI
- **Testing**: NUnit, Moq, FluentAssertions

### Key Packages

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore | 10.0.3 | ORM |
| Microsoft.EntityFrameworkCore.Sqlite | 10.0.3 | Database Provider |
| Swashbuckle.AspNetCore | 10.1.3 | Swagger UI |
| NUnit | 4.4.0 | Testing Framework |
| Moq | 4.20.72 | Mocking Library |
| FluentAssertions | 8.8.0 | Test Assertions |

## 📦 Database Schema

The system manages the following entities:

- **Teachers** - Personal data, subjects taught, assigned classroom
- **Students** - Personal data, class assignment, grades
- **Classes** - Class names and status
- **Subjects** - Academic subject names
- **Grades** - Student grades per subject per term (2-5 scale)

See `docs/db_diagram.png` for the complete ER diagram.

## 🚀 Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- SQLite (included with .NET)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd course_work
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure the database**
   
   The default connection string is configured in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=school.db"
   }
   ```

4. **Apply migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the API**
   - Swagger UI: `https://localhost:5001/swagger` or `http://localhost:5000/swagger`
   - API endpoints: `https://localhost:5001/api/*`

## 📡 API Endpoints

### Teachers
- `POST /api/teachers` - Add a new teacher
- `GET /api/teachers` - Get all teachers
- `DELETE /api/teachers/{id}` - Remove a teacher

### Students
- `POST /api/students` - Add a new student
- `GET /api/students` - Get all students
- `DELETE /api/students/{id}` - Remove a student

### Grades
- `POST /api/grades` - Add grades (by class or by student)
- `PUT /api/grades` - Update grade (before final submission)

## 🧪 Running Tests

```bash
dotnet test
```

## 📁 Project Structure

```
course_work/
├── src/                          # Source code
│   ├── SchoolManagement.Application/
│   ├── SchoolManagement.Domain/
│   ├── SchoolManagement.Infrastructure/
│   ├── SchoolManagement.Presentation/
│   ├── SchoolManagement.Middleware/
│   └── SchoolManagement.Tests/
├── docs/                         # Documentation
│   ├── db_diagram.png           # Database ER diagram
│   ├── db_diagram.puml          # PlantUML source
│   ├── extracted_task.md        # Technical requirements
│   └── plaint_task.md           # Original task description
├── Migrations/                   # EF Core migrations
├── appsettings.json             # Configuration
├── Program.cs                   # Application entry point
└── course_work.csproj           # Project file
```

## 📝 Business Rules

1. **Teacher Constraints**
   - One teacher can be assigned to only one classroom
   - One teacher can teach unlimited subjects

2. **Grade Constraints**
   - Grades must be integers from 2 to 5
   - Grades cannot be modified after submission

3. **Student Constraints**
   - Each student belongs to exactly one class
   - Class transfer is done through student profile editing

4. **Class & Subject Constraints**
   - Classes exist independently of student enrollment
   - Subject list is dynamic and user-defined

## 📄 License

This project is licensed under the terms specified in the [LICENSE](LICENSE) file.

## 📞 Support

For issues and questions, please refer to the documentation in the `docs/` folder or create an issue in the repository.
