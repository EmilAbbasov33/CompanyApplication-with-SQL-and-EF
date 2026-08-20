# 🏢 CompanyApplication

A console-based company management system built with **C#**, **Entity Framework Core**, and **SQL Server**.  
Manages departments and employees with full CRUD operations, validation, and a rich interactive UI powered by **Spectre.Console**.

---

## 🛠️ Tech Stack

| Technology | Purpose |
|---|---|
| C# / .NET | Core language |
| Entity Framework Core | ORM & database migrations |
| SQL Server (LocalDB/Express) | Database |
| AutoMapper | Entity ↔ DTO mapping |
| Spectre.Console | Interactive console UI |

---

## 🏗️ Architecture

The project follows a **Layered Architecture**:

```
CompanyApplication     → UI / Entry point (Console + Controllers)
Service                → Business Logic (Services, DTOs, Validation)
Repository             → Data Access (Repositories, DbContext)
Domain                 → Core Entities
```

---

## 🗂️ Project Structure

```
CompanyApplication-with-SQL-and-EF/
│
├── Domain/
│   └── Entities/
│       ├── BaseEntity.cs
│       ├── Department.cs
│       └── Employee.cs
│
├── Repository/
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Migrations/
│   └── Repositories/
│       ├── Interfaces/
│       │   ├── IBaseRepository.cs
│       │   ├── IDepartmentRepository.cs
│       │   └── IEmployeeRepository.cs
│       ├── BaseRepository.cs
│       ├── DepartmentRepository.cs
│       └── EmployeeRepository.cs
│
├── Service/
│   ├── DTOs/
│   ├── Delegates/
│   ├── Enums/
│   ├── Exceptions/
│   ├── Helpers/
│   ├── Profiles/
│   └── Services/
│       ├── Interfaces/
│       ├── DepartmentService.cs
│       └── EmployeeService.cs
│
└── CompanyApplication/
    ├── Controller/
    │   ├── DepartmentController.cs
    │   └── EmployeeController.cs
    └── Program.cs
```

---

## 🔗 Entity Relationship

```
Department  ──────────────<  Employee
   (1)                          (N)
```

- One **Department** can have many **Employees**
- One **Employee** belongs to exactly one **Department**

---

## ✨ Features

### Department Management
- ✅ Create Department
- ✅ Update Department (name, capacity)
- ✅ Delete Department (cascades to employees)
- ✅ Get Department by ID
- ✅ Get All Departments
- ✅ Search Departments by Name
- ✅ Sort Departments by Capacity (Ascending / Descending)

### Employee Management
- ✅ Create Employee
- ✅ Update Employee
- ✅ Delete Employee
- ✅ Get Employee by ID
- ✅ Get Employees by Age
- ✅ Get Employees by Department ID
- ✅ Get All Employees by Department Name
- ✅ Search Employees (by name or surname)
- ✅ Get Total Employee Count
- ✅ Get All Employees

---

## ▶️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server or SQL Server Express

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/EmilAbbasov33/CompanyApplication-with-SQL-and-EF.git
   cd CompanyApplication-with-SQL-and-EF
   ```

2. **Update connection string** in `Repository/Data/AppDbContext.cs`
   ```csharp
   optionsBuilder.UseSqlServer("Your_Connection_String_Here");
   ```

3. **Run the application**
   ```bash
   dotnet run --project CompanyApplication
   ```
   Database migrations will be applied automatically on first run.

---

## 📐 Design Patterns Used

- **Repository Pattern** — Generic `BaseRepository<T>` for reusable CRUD
- **DTO Pattern** — Clean separation between entities and API/UI layer
- **Service Layer Pattern** — Business logic isolated from data access
- **Delegate Pattern** — Reusable validation rules via delegates

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).
