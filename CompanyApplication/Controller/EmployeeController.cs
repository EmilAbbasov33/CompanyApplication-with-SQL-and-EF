using Domain.Entities;
using Service.Exceptions;
using Service.Services.Interfaces;
using Spectre.Console;

namespace CompanyApplication.Controller
{
    public class EmployeeController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        private void PrintError(string message)
        {
            var panel = new Panel($"[red]XƏTA: {message}[/]").BorderColor(Color.Red);
            AnsiConsole.Write(panel);
        }

        private void PrintSuccess(string message)
        {
            AnsiConsole.MarkupLine($"[green]✔ {message}[/]");
        }

        private void PrintEmployees(IEnumerable<Employee> employees, string emptyMessage = "[yellow]Siyahı boşdur.[/]")
        {
            if (!employees.Any())
            {
                AnsiConsole.MarkupLine(emptyMessage);
                return;
            }

            var table = new Table().BorderColor(Color.Cyan).Border(TableBorder.Rounded).Expand();
            table.AddColumn("[cyan]ID[/]");
            table.AddColumn("[cyan]Ad Soyad[/]");
            table.AddColumn("[cyan]Yaş[/]");
            table.AddColumn("[cyan]Ünvan[/]");
            table.AddColumn("[cyan]Department[/]");

            foreach (var emp in employees)
            {
                table.AddRow(emp.Id.ToString(), $"{emp.Name} {emp.Surname}", emp.Age.ToString(), emp.Address, emp.Department?.Name ?? "Yoxdur");
            }

            AnsiConsole.Write(table);
        }

        public void Create(Employee employee)
        {
            try
            {
                AnsiConsole.Status().Start("Yaradılır...", ctx => {
                    _employeeService.Create(employee);
                });
                PrintSuccess("Employee uğurla yaradıldı.");
            }
            catch (ValidationException ex)
            {
                PrintError(ex.Message);
            }
            catch (DepartmentNotFoundException ex)
            {
                PrintError(ex.Message);
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }

        public void Update(Employee employee)
        {
            try
            {
                AnsiConsole.Status().Start("Yenilənir...", ctx => {
                    _employeeService.Update(employee);
                });
                PrintSuccess("Employee uğurla yeniləndi.");
            }
            catch (EmployeeNotFoundException ex)
            {
                PrintError(ex.Message);
            }
            catch (DepartmentNotFoundException ex)
            {
                PrintError(ex.Message);
            }
            catch (ValidationException ex)
            {
                PrintError(ex.Message);
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }

        public void Delete(int id)
        {
            try
            {
                AnsiConsole.Status().Start("Silinir...", ctx => {
                    _employeeService.Delete(id);
                });
                PrintSuccess("Employee uğurla silindi.");
            }
            catch (EmployeeNotFoundException ex)
            {
                PrintError(ex.Message);
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }

        public void PrintById(int id)
        {
            try
            {
                var emp = _employeeService.GetById(id);
                var table = new Table().BorderColor(Color.Cyan).Border(TableBorder.Rounded).Expand();
                table.AddColumn("[cyan]ID[/]");
                table.AddColumn("[cyan]Ad Soyad[/]");
                table.AddColumn("[cyan]Yaş[/]");
                table.AddColumn("[cyan]Ünvan[/]");
                table.AddColumn("[cyan]Department[/]");
                table.AddRow(emp.Id.ToString(), $"{emp.Name} {emp.Surname}", emp.Age.ToString(), emp.Address, emp.Department?.Name ?? "Yoxdur");
                AnsiConsole.Write(table);
            }
            catch (EmployeeNotFoundException ex)
            {
                PrintError(ex.Message);
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }

        public void PrintByAge(int age)
        {
            try
            {
                var employees = _employeeService.GetByAge(age);
                PrintEmployees(employees, "[red]Axtarışa uyğun nəticə tapılmadı.[/]");
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }

        public void PrintByDepartmentId(int departmentId)
        {
            try
            {
                var employees = _employeeService.GetByDepartmentId(departmentId);
                PrintEmployees(employees, "[red]Axtarışa uyğun nəticə tapılmadı.[/]");
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }

        public void PrintAllByDepartmentName(string departmentName)
        {
            try
            {
                var employees = _employeeService.GetAllByDepartmentName(departmentName);
                PrintEmployees(employees, "[red]Axtarışa uyğun nəticə tapılmadı.[/]");
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }

        public bool Search(string text)
        {
            try
            {
                var employees = _employeeService.Search(text);
                if (!employees.Any())
                {
                    AnsiConsole.MarkupLine("[red]Axtarışa uyğun nəticə tapılmadı. Zəhmət olmasa yenidən cəhd edin.[/]");
                    return false;
                }
                PrintEmployees(employees);
                return true;
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
                return true;
            }
        }

        public void PrintCount()
        {
            try
            {
                var count = _employeeService.GetCount();
                var panel = new Panel($"Ümumi işçi sayı: [yellow]{count}[/]").BorderColor(Color.Green);
                AnsiConsole.Write(panel);
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }

        public void PrintAll()
        {
            try
            {
                var employees = _employeeService.GetAll();
                PrintEmployees(employees);
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }
    }
}
