using Service.DTOs;
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

        private async Task PrintDepartmentsAsync(IEnumerable<DepartmentDto> departments, string emptyMessage = "[yellow]Siyahı boşdur.[/]")
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
                var employees = await _employeeService.GetByDepartmentIdAsync(dept.Id);
                int currentEmployees = employees.Count();
                table.AddRow(dept.Id.ToString(), dept.Name, dept.Capacity.ToString(), currentEmployees.ToString());
            }

            AnsiConsole.Write(table);
        }

        public async Task CreateAsync(DepartmentCreateDto dto)
        {
            try
            {
                await AnsiConsole.Status().StartAsync("Yaradılır...", async ctx => {
                    await _departmentService.CreateAsync(dto);
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

        public async Task UpdateAsync(DepartmentUpdateDto dto)
        {
            try
            {
                await AnsiConsole.Status().StartAsync("Yenilənir...", async ctx => {
                    await _departmentService.UpdateAsync(dto);
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

        public async Task DeleteAsync(int id)
        {
            try
            {
                await AnsiConsole.Status().StartAsync("Silinir...", async ctx => {
                    await _departmentService.DeleteAsync(id);
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

        public async Task PrintByIdAsync(int id)
        {
            try
            {
                var dept = await _departmentService.GetByIdAsync(id);
                var table = new Table().BorderColor(Color.Cyan).Border(TableBorder.Rounded).Expand();
                table.AddColumn("[cyan]ID[/]");
                table.AddColumn("[cyan]Name[/]");
                table.AddColumn("[cyan]Capacity[/]");
                table.AddColumn("[cyan]Current Employees[/]");
                
                var employees = await _employeeService.GetByDepartmentIdAsync(dept.Id);
                int currentEmployees = employees.Count();
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

        public async Task PrintAllAsync()
        {
            try
            {
                var departments = await _departmentService.GetAllAsync();
                await PrintDepartmentsAsync(departments);
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }

        public async Task<bool> SearchAsync(string text)
        {
            try
            {
                var departments = await _departmentService.SearchAsync(text);
                if (!departments.Any())
                {
                    AnsiConsole.MarkupLine("[red]Axtarışa uyğun nəticə tapılmadı. Zəhmət olmasa yenidən cəhd edin.[/]");
                    return false;
                }
                await PrintDepartmentsAsync(departments);
                return true;
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
                return true;
            }
        }

        public async Task SortByCapacityAsync(SortDirection direction)
        {
            try
            {
                var departments = await _departmentService.SortByCapacityAsync(direction);
                await PrintDepartmentsAsync(departments);
            }
            catch (Exception ex)
            {
                PrintError($"Gözlənilməz xəta: {ex.Message}");
            }
        }
    }
}
