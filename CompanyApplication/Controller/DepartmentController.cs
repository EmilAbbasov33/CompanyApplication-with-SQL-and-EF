using Domain.Entities;
using Service.Enums;
using Service.Exceptions;
using Service.Services.Interfaces;
using Spectre.Console;

namespace CompanyApplication.Controller
{
    public class DepartmentController
    {
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;
        public DepartmentController(IDepartmentService departmentService, IEmployeeService employeeService)
        {
            _departmentService = departmentService;
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

        private void PrintDepartments(IEnumerable<Department> departments, string emptyMessage = "[yellow]Siyahı boşdur.[/]")
        {
            if (!departments.Any())
            {
                AnsiConsole.MarkupLine(emptyMessage);
                return;
            }

            var table = new Table().BorderColor(Color.Cyan).Border(TableBorder.Rounded).Expand();
            table.AddColumn("[cyan]ID[/]");
            table.AddColumn("[cyan]Name[/]");
            table.AddColumn("[cyan]Capacity[/]");
            table.AddColumn("[cyan]Current Employees[/]");

            foreach (var dept in departments)
            {
                int currentEmployees = _employeeService.GetByDepartmentId(dept.Id).Count();
                table.AddRow(dept.Id.ToString(), dept.Name, dept.Capacity.ToString(), currentEmployees.ToString());
            }

            AnsiConsole.Write(table);
        }

        public void Create(Department department)
        {
            try
            {
                AnsiConsole.Status().Start("Yaradılır...", ctx => {
                    _departmentService.Create(department);
                });
                PrintSuccess("Department uğurla yaradıldı.");
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

        public void Update(Department department)
        {
            try
            {
                AnsiConsole.Status().Start("Yenilənir...", ctx => {
                    _departmentService.Update(department);
                });
                PrintSuccess("Department uğurla yeniləndi.");
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
                    _departmentService.Delete(id);
                });
                PrintSuccess("Department uğurla silindi.");
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

        public void PrintById(int id)
        {
            try
            {
                var dept = _departmentService.GetById(id);
                var table = new Table().BorderColor(Color.Cyan).Border(TableBorder.Rounded).Expand();
                table.AddColumn("[cyan]ID[/]");
                table.AddColumn("[cyan]Name[/]");
                table.AddColumn("[cyan]Capacity[/]");
                table.AddColumn("[cyan]Current Employees[/]");
                
                int currentEmployees = _employeeService.GetByDepartmentId(dept.Id).Count();
                table.AddRow(dept.Id.ToString(), dept.Name, dept.Capacity.ToString(), currentEmployees.ToString());
                AnsiConsole.Write(table);
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

        public void PrintAll()
        {
            try
            {
                var departments = _departmentService.GetAll();
                PrintDepartments(departments);
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
                var departments = _departmentService.Search(text);
                if (!departments.Any())
                {
                    AnsiConsole.MarkupLine("[red]Axtarışa uyğun nəticə tapılmadı. Zəhmət olmasa yenidən cəhd edin.[/]");
                    return false;
                }
                PrintDepartments(departments);
                return true;
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
                return true;
            }
        }

        public void SortByCapacity(SortDirection direction)
        {
            try
            {
                var departments = _departmentService.SortByCapacity(direction);
                PrintDepartments(departments);
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }
    }
}
