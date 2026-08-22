using AutoMapper;
using CompanyApplication.Controller;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories;
using Service.AutoMapper;
using Service.Delegates;
using Service.DTOs;
using Service.Enums;
using Service.Helpers.Constants;
using Service.Services;
using Service.Services.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Text;

Console.OutputEncoding = System.Text.Encoding.UTF8;

AnsiConsole.Status().Start("Verilənlər bazası yoxlanılır...", ctx =>
{
    using (var dbContext = new AppDbContext())
    {
        dbContext.Database.Migrate();
    }
});

var departmentRepository = new DepartmentRepository();
var employeeRepository = new EmployeeRepository();
var mapper = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingProfile());
}, Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance).CreateMapper();
var departmentService = new DepartmentService(departmentRepository, employeeRepository, mapper);
var employeeService = new EmployeeService(employeeRepository, departmentRepository, mapper);
var departmentController = new DepartmentController(departmentService, employeeService);
var employeeController = new EmployeeController(employeeService);

var menuItems = new List<(string Label, MenuOption Option, string? Section)>
{
    ("1. Create Department", MenuOption.CreateDepartment, "DEPARTMENT"),
    ("2. Update Department", MenuOption.UpdateDepartment, null),
    ("3. Delete Department", MenuOption.DeleteDepartment, null),
    ("4. Get Department By Id", MenuOption.GetDepartmentById, null),
    ("5. Get All Departments", MenuOption.GetAllDepartments, null),
    ("6. Search Departments By Name", MenuOption.SearchDepartments, null),
    ("7. Sort Departments By Capacity", MenuOption.SortDepartmentsByCapacity, null),
    ("8. Create Employee", MenuOption.CreateEmployee, "EMPLOYEE"),
    ("9. Update Employee", MenuOption.UpdateEmployee, null),
    ("10. Get Employee By Id", MenuOption.GetEmployeeById, null),
    ("11. Delete Employee", MenuOption.DeleteEmployee, null),
    ("12. Get Employees By Age", MenuOption.GetEmployeesByAge, null),
    ("13. Get Employees By DepartmentId", MenuOption.GetEmployeesByDepartmentId, null),
    ("14. Get All Employees By DepartmentName", MenuOption.GetAllEmployeesByDepartmentName, null),
    ("15. Search Employees (by name/surname)", MenuOption.SearchEmployees, null),
    ("16. Get All Employees Count", MenuOption.GetAllEmployeesCount, null),
    ("17. GetAll Employees", MenuOption.GetAllEmployees, "SYSTEM"),
    ("0. Çıxış", MenuOption.Exit, null)
};

var previewTexts = new Dictionary<MenuOption, (string Title, string Desc)>
{
    [MenuOption.CreateDepartment] = ("Departament Yaratmaq", "Sistemə yeni bir departament əlavə edin."),
    [MenuOption.UpdateDepartment] = ("Departament Yeniləmək", "Mövcud departamentin adını və ya tutumunu\n(capacity) dəyişmək üçün istifadə olunur."),
    [MenuOption.DeleteDepartment] = ("Departament Silmək", "Mövcud departamenti sistemdən tamamilə silir.\n[red]Diqqət:[/] İçində işçi olan departamenti\nsilsəniz işçi məlumatları da silinəcək."),
    [MenuOption.GetDepartmentById] = ("ID-yə görə Departament", "Spesifik bir departamentin tutumu və\ndetallarına baxmaq üçün onun ID-sini\ndaxil edirsiniz."),
    [MenuOption.GetAllDepartments] = ("Bütün Departamentlər", "Sistemdəki bütün departamentlərin cədvəl\nformasında tam siyahısını göstərir."),
    [MenuOption.SearchDepartments] = ("Departament Axtarışı", "Adında daxil etdiyiniz söz olan bütün\ndepartamentləri gətirir."),
    [MenuOption.SortDepartmentsByCapacity] = ("Tutumuna Görə Sıralama", "Departamentləri tutumlarına (capacity) görə\nartan və ya azalan ardıcıllıqla göstərir."),
    [MenuOption.CreateEmployee] = ("İşçi Yaratmaq", "Sistemə yeni bir işçi əlavə edin və onu müəyyən\nbir departamentə təyin edin."),
    [MenuOption.UpdateEmployee] = ("İşçi Yeniləmək", "İşçinin adını, soyadını, yaşını, ünvanını və ya\naid olduğu departamenti dəyişir."),
    [MenuOption.GetEmployeeById] = ("ID-yə görə İşçi", "İşçinin bütün detallarına ID-si vasitəsilə\nbaxın."),
    [MenuOption.DeleteEmployee] = ("İşçi Silmək", "Seçilmiş işçini sistemdən tamamilə silir."),
    [MenuOption.GetEmployeesByAge] = ("Yaşına görə İşçilər", "Daxil edilən yaşda olan bütün işçilərin\nsiyahısını verir."),
    [MenuOption.GetEmployeesByDepartmentId] = ("Departament ID-yə görə İşçilər", "Verilmiş Departament ID-sinə aid olan bütün\nişçiləri cədvəldə göstərir."),
    [MenuOption.GetAllEmployeesByDepartmentName] = ("Şöbə adına görə İşçilər", "Departamentin adını yazaraq oradakı bütün\nişçiləri görə bilərsiniz."),
    [MenuOption.SearchEmployees] = ("İşçi Axtarışı", "Adında və ya soyadında daxil etdiyiniz mətni\nehtiva edən işçiləri tapır."),
    [MenuOption.GetAllEmployeesCount] = ("İşçi Statistikası", "Şirkətdə çalışan işçilərin ümumi statistikasını\nverir."),
    [MenuOption.GetAllEmployees] = ("Bütün İşçilər", "Şirkətdəki bütün işçilərin tam və detallı\ncədvəlini ekrana çap edir."),
    [MenuOption.Exit] = ("Çıxış", "Proqramdan təhlükəsiz şəkildə çıxmaq üçün\nistifadə olunur.")
};

while (true)
{
    Console.Clear();
    int selectedIndex = 0;
    MenuOption? chosenOption = null;
    string searchBuffer = "";

    AnsiConsole.Live(new Panel("Yüklənir..."))
        .Start(ctx =>
        {
            while (true)
            {
                var layoutTable = new Table().NoBorder().HideHeaders().Expand();
                layoutTable.AddColumn(new TableColumn("Left").Width(55));
                layoutTable.AddColumn(new TableColumn("Right"));

                var leftContent = new StringBuilder();
                for (int i = 0; i < menuItems.Count; i++)
                {
                    if (menuItems[i].Section != null)
                        leftContent.AppendLine($"[yellow]=== {menuItems[i].Section} ===[/]");

                    if (i == selectedIndex)
                    {
                        if (menuItems[i].Option == MenuOption.Exit)
                            leftContent.AppendLine($"[bold red]> {menuItems[i].Label}[/]");
                        else
                            leftContent.AppendLine($"[green]> {menuItems[i].Label}[/]");
                    }
                    else
                    {
                        leftContent.AppendLine($"  {menuItems[i].Label}");
                    }
                }

                var leftPanel = new Panel(leftContent.ToString().TrimEnd())
                    .BorderColor(Color.White)
                    .Padding(1, 0, 1, 0)
                    .Header("[cyan] İcra etmək üçün seçin: [/]");

                var rightContent = GetPreviewForOption(menuItems[selectedIndex].Option, previewTexts, departmentService, employeeService);
                var topRightPanel = new Panel(rightContent)
                    .Expand()
                    .BorderColor(Color.White)
                    .Padding(2, 0, 2, 0)
                    .Header("[yellow] Məlumat [/]");

                var bottomContent = GetStatsPanelContent(departmentService, employeeService);
                var bottomRightPanel = new Panel(bottomContent)
                    .Expand()
                    .BorderColor(Color.White)
                    .Padding(2, 0, 2, 0)
                    .Header("[green] Statistika [/]");

                var rightColumn = new Rows(topRightPanel, bottomRightPanel);
                layoutTable.AddRow(leftPanel, rightColumn);

                var headerTableInner = new Table().NoBorder().HideHeaders().Expand();
                headerTableInner.AddColumn(new TableColumn("Left").LeftAligned());
                headerTableInner.AddColumn(new TableColumn("Right").RightAligned());
                headerTableInner.AddRow("[bold white]CompanyApplication[/]", "[bold white]/codeacademy[/]");

                var headerPanel = new Panel(headerTableInner)
                    .Expand()
                    .BorderColor(Color.White)
                    .Border(BoxBorder.Square);

                var mainLayout = new Rows(
                    headerPanel,
                    layoutTable,
                    new Markup("[grey](Ox düymələri və ya rəqəm yazaraq hərəkət edin. Seçmək üçün Enter)[/]")
                );

                ctx.UpdateTarget(mainLayout);

                var key = Console.ReadKey(true);

                if (char.IsDigit(key.KeyChar))
                {
                    searchBuffer += key.KeyChar;
                    int foundIndex = menuItems.FindIndex(m => m.Label.StartsWith(searchBuffer + "."));
                    if (foundIndex != -1)
                        selectedIndex = foundIndex;
                    else
                    {
                        searchBuffer = key.KeyChar.ToString();
                        foundIndex = menuItems.FindIndex(m => m.Label.StartsWith(searchBuffer + "."));
                        if (foundIndex != -1) selectedIndex = foundIndex;
                    }
                }
                else
                {
                    searchBuffer = "";
                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        selectedIndex--;
                        if (selectedIndex < 0) selectedIndex = menuItems.Count - 1;
                    }
                    else if (key.Key == ConsoleKey.DownArrow)
                    {
                        selectedIndex++;
                        if (selectedIndex >= menuItems.Count) selectedIndex = 0;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        chosenOption = menuItems[selectedIndex].Option;
                        break;
                    }
                }
            }
        });

    if (chosenOption == null) continue;
    MenuOption selectedOption = chosenOption.Value;

    if (selectedOption == MenuOption.Exit)
    {
        AnsiConsole.MarkupLine("[red]Çıxılır...[/]");
        break;
    }

    Console.Clear();

    var selectedLabelItem = menuItems.FirstOrDefault(m => m.Option == selectedOption).Label;
    if (selectedLabelItem != null)
    {
        var titleText = selectedLabelItem.Contains(".") ? selectedLabelItem.Split('.', 2)[1].Trim() : selectedLabelItem;
        AnsiConsole.Write(new Rule($"[bold cyan]{titleText}[/]").RuleStyle("yellow").LeftJustified());
        AnsiConsole.WriteLine();
    }

    if (selectedOption != MenuOption.SortDepartmentsByCapacity && selectedOption != MenuOption.Exit)
    {
        AnsiConsole.MarkupLine("[grey](İstənilən vaxt əsas menyuya qayıtmaq üçün [bold white]ESC[/] düyməsinə basın)[/]\n");
    }

    bool shouldSkipWait = false;

    switch (selectedOption)
    {
        case MenuOption.CreateDepartment:
            shouldSkipWait = await HandleCreateDepartment(departmentController, departmentService);
            break;
        case MenuOption.UpdateDepartment:
            shouldSkipWait = await HandleUpdateDepartment(departmentController, departmentService);
            break;
        case MenuOption.DeleteDepartment:
            shouldSkipWait = await HandleDeleteDepartment(departmentController, departmentService);
            break;
        case MenuOption.GetDepartmentById:
            shouldSkipWait = await HandleGetDepartmentById(departmentController, departmentService);
            break;
        case MenuOption.GetAllDepartments:
            await departmentController.PrintAllAsync();
            break;
        case MenuOption.SearchDepartments:
            await HandleSearchDepartments(departmentController);
            break;
        case MenuOption.SortDepartmentsByCapacity:
            await HandleSortDepartmentsByCapacity(departmentController);
            break;
        case MenuOption.CreateEmployee:
            shouldSkipWait = await HandleCreateEmployee(employeeController, departmentController, departmentService, employeeService);
            break;
        case MenuOption.UpdateEmployee:
            shouldSkipWait = await HandleUpdateEmployee(employeeController, departmentService, employeeService);
            break;
        case MenuOption.GetEmployeeById:
            shouldSkipWait = await HandleGetEmployeeById(employeeController, employeeService);
            break;
        case MenuOption.DeleteEmployee:
            shouldSkipWait = await HandleDeleteEmployee(employeeController, employeeService);
            break;
        case MenuOption.GetEmployeesByAge:
            shouldSkipWait = await HandleGetEmployeesByAge(employeeController);
            break;
        case MenuOption.GetEmployeesByDepartmentId:
            shouldSkipWait = await HandleGetEmployeesByDepartmentId(employeeController, departmentController, departmentService);
            break;
        case MenuOption.GetAllEmployeesByDepartmentName:
            shouldSkipWait = await HandleGetAllEmployeesByDepartmentName(employeeController, departmentController, departmentService);
            break;
        case MenuOption.SearchEmployees:
            await HandleSearchEmployees(employeeController);
            break;
        case MenuOption.GetAllEmployeesCount:
            await employeeController.PrintCountAsync();
            break;
        case MenuOption.GetAllEmployees:
            await employeeController.PrintAllAsync();
            break;
    }

    if (shouldSkipWait) continue;

    AnsiConsole.MarkupLine("\n[grey]Davam etmək üçün istənilən düyməyə basın...[/]");
    Console.ReadKey(true);
}

static Func<string, ValidationResult> IdExistsValidator(Func<int, object> getById)
    => idStr =>
    {
        if (!int.TryParse(idStr, out int id))
            return ValidationResult.Error("[red]Düzgün rəqəm daxil edin[/]");
        try { getById(id); return ValidationResult.Success(); }
        catch (Exception ex) { return ValidationResult.Error($"[red]{ex.InnerException?.Message ?? ex.Message}[/]"); }
    };

static Func<string, ValidationResult> ValidName(string label)
    => value => NameValidator.IsValidName(value)
        ? ValidationResult.Success()
        : ValidationResult.Error($"[red]{label} yalnız hərflərdən ibarət olmalıdır[/]");

static Func<string, ValidationResult> ValidAge()
    => ageStr =>
    {
        if (!int.TryParse(ageStr, out int age)) return ValidationResult.Error("[red]Düzgün rəqəm daxil edin[/]");
        if (age < 18 || age > 60) return ValidationResult.Error("[red]Yaş 18 ilə 60 arasında olmalıdır[/]");
        return ValidationResult.Success();
    };

static bool EnsureAny(bool exists, string emptyMessage)
{
    if (exists) return true;
    AnsiConsole.MarkupLine($"[yellow]{emptyMessage}[/]");
    return false;
}

static IRenderable PadToLines(StringBuilder content, int targetLines)
{
    int currentLines = content.ToString().Split('\n').Length;
    for (int i = currentLines; i < targetLines; i++)
        content.AppendLine();
    return new Markup(content.ToString());
}

static async Task<bool> HandleDelete(
    bool anyExists,
    string emptyMessage,
    Func<Task> printAll,
    string idLabel,
    Func<int, object> getById,
    string confirmMessage,
    Func<int, Task> delete)
{
    if (!EnsureAny(anyExists, emptyMessage)) return false;
    await printAll();
    AnsiConsole.MarkupLine("");
    string idStr = GetStringInput($"[yellow]{idLabel}[/]", IdExistsValidator(getById));
    if (idStr == "-1") return true;

    if (AnsiConsole.Confirm($"\n[red]{confirmMessage}[/]"))
        await delete(int.Parse(idStr));
    return false;
}

static async Task<bool> HandleGetById(bool anyExists, string emptyMessage, string idLabel, Func<int, object> getById, Func<int, Task> printById)
{
    if (!EnsureAny(anyExists, emptyMessage)) return false;
    string idStr = GetStringInput($"[yellow]{idLabel}[/]", IdExistsValidator(getById));
    if (idStr == "-1") return true;
    await printById(int.Parse(idStr));
    return false;
}

static async Task<bool> HandleCreateDepartment(DepartmentController departmentController, IDepartmentService departmentService)
{
    string dName = GetStringInput("[yellow]Department adı:[/]", name =>
    {
        var all = departmentService.GetAll().GetAwaiter().GetResult();
        if (all.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return ValidationResult.Error($"[red]{ValidationConstants.DepartmentNameExistsMessage}[/]");
        return ValidationResult.Success();
    });
    if (dName == "-1") return true;

    string dCapStr = GetStringInput("[yellow]Capacity:[/]", cap =>
    {
        if (!int.TryParse(cap, out int pCap)) return ValidationResult.Error("[red]Düzgün rəqəm daxil edin[/]");
        if (pCap < 0) return ValidationResult.Error("[red]Capacity mənfi ola bilməz[/]");
        return ValidationResult.Success();
    });
    if (dCapStr == "-1") return true;

    await departmentController.CreateAsync(new DepartmentCreateDto { Name = dName, Capacity = int.Parse(dCapStr) });
    return false;
}

static async Task<bool> HandleUpdateDepartment(DepartmentController departmentController, IDepartmentService departmentService)
{
    var allDepts = await departmentService.GetAll();
    if (!EnsureAny(allDepts.Any(), "Sistemdə heç bir departament yoxdur! Yeniləmək üçün əvvəlcə departament yaradın.")) return false;
    await departmentController.PrintAllAsync();
    AnsiConsole.MarkupLine("");
    string updateDIdStr = GetStringInput("[yellow]Yenilənəcək Department ID:[/]", IdExistsValidator(id => departmentService.GetById(id).GetAwaiter().GetResult()!));
    if (updateDIdStr == "-1") return true;
    int updateDId = int.Parse(updateDIdStr);

    AnsiConsole.MarkupLine("[grey]Diqqət: Boş buraxsanız köhnə dəyər saxlanılacaq.[/]");
    string newDName = GetStringInput("[yellow]Yeni Department adı:[/]", name =>
    {
        var existingD = departmentService.GetById(updateDId).GetAwaiter().GetResult()!;
        var all = departmentService.GetAll().GetAwaiter().GetResult();
        if (!existingD!.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
            all.Any(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return ValidationResult.Error($"[red]{ValidationConstants.DepartmentNameExistsMessage}[/]");
        return ValidationResult.Success();
    }, allowEmpty: true);
    if (newDName == "-1") return true;

    string capInput = GetStringInput("[yellow]Yeni Capacity:[/]", cap =>
    {
        if (!int.TryParse(cap, out int parsedCap)) return ValidationResult.Error("[red]Zəhmət olmasa düzgün rəqəm daxil edin[/]");
        if (parsedCap < 0) return ValidationResult.Error("[red]Capacity mənfi ola bilməz[/]");
        return ValidationResult.Success();
    }, allowEmpty: true);
    if (capInput == "-1") return true;

    int? newDCapacity = string.IsNullOrEmpty(capInput) ? null : int.Parse(capInput);
    var updateDto = new DepartmentUpdateDto { Id = updateDId, Name = string.IsNullOrEmpty(newDName) ? null : newDName, Capacity = newDCapacity };
    await departmentController.UpdateAsync(updateDto);
    return false;
}

static async Task<bool> HandleDeleteDepartment(DepartmentController departmentController, IDepartmentService departmentService)
{
    var allDepts = await departmentService.GetAll();
    return await HandleDelete(
        allDepts.Any(),
        "Sistemdə heç bir departament yoxdur!",
        async () => { await departmentController.PrintAllAsync(); },
        "Silinəcək Department ID:",
        id => departmentService.GetById(id).GetAwaiter().GetResult()!,
        "Bu departamenti silmək istədiyinizə əminsiniz?",
        async id => { await departmentController.DeleteAsync(id); });
}

static async Task<bool> HandleGetDepartmentById(DepartmentController departmentController, IDepartmentService departmentService)
{
    var allDepts = await departmentService.GetAll();
    return await HandleGetById(
        allDepts.Any(),
        "Sistemdə heç bir departament yoxdur!",
        "Department ID:",
        id => departmentService.GetById(id).GetAwaiter().GetResult()!,
        async id => { await departmentController.PrintByIdAsync(id); });
}

static async Task HandleSearchDepartments(DepartmentController departmentController)
{
    while (true)
    {
        string searchD = GetStringInput("[yellow]Axtarış mətni (boş qoysanız hamısı gələcək):[/]", null, true);
        if (searchD == "-1") break;

        bool found = await departmentController.SearchAsync(searchD);
        if (found) break;
    }
}

static async Task HandleSortDepartmentsByCapacity(DepartmentController departmentController)
{
    var sortType = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[yellow]Sıralama növü:[/]")
            .AddChoices(new[] { "Artan (Ascending)", "Azalan (Descending)", "[red]Geri Qayıt[/]" }));

    if (sortType.Contains("Geri Qayıt")) return;
    if (sortType == "Artan (Ascending)")
        await departmentController.SortByCapacityAsync(SortDirection.Ascending);
    else
        await departmentController.SortByCapacityAsync(SortDirection.Descending);
}

static async Task<bool> HandleCreateEmployee(EmployeeController employeeController, DepartmentController departmentController, IDepartmentService departmentService, IEmployeeService employeeService)
{
    var allDepts = await departmentService.GetAll();
    if (!EnsureAny(allDepts.Any(), "Sistemdə heç bir departament yoxdur! İşçi əlavə etmək üçün əvvəlcə departament yaratmalısınız.")) return false;
    await departmentController.PrintAllAsync();
    AnsiConsole.MarkupLine("");
    string eDeptIdStr = GetStringInput("[yellow]Department ID:[/]", idStr =>
    {
        if (!int.TryParse(idStr, out int id)) return ValidationResult.Error("[red]Düzgün rəqəm daxil edin[/]");
        try
        {
            var dept = departmentService.GetById(id).GetAwaiter().GetResult()!;
            if (dept!.Capacity == 0) return ValidationResult.Error($"[red]{string.Format(ValidationConstants.DepartmentCapacityZeroMessage, dept!.Name)}[/]");
            int currentCount = employeeService.GetByDepartmentIdAsync(id).GetAwaiter().GetResult().Count();
            if (currentCount >= dept!.Capacity) return ValidationResult.Error($"[red]{string.Format(ValidationConstants.DepartmentFullMessage, dept!.Name, dept!.Capacity)}[/]");
            return ValidationResult.Success();
        }
        catch (Exception ex) { return ValidationResult.Error($"[red]{ex.InnerException?.Message ?? ex.Message}[/]"); }
    });
    if (eDeptIdStr == "-1") return true;
    int eDeptId = int.Parse(eDeptIdStr);

    string eName = GetStringInput("[yellow]Employee adı:[/]", ValidName("Ad"));
    if (eName == "-1") return true;

    string eSurname = GetStringInput("[yellow]Employee soyadı:[/]", ValidName("Soyad"));
    if (eSurname == "-1") return true;

    string eAgeStr = GetStringInput("[yellow]Yaşı:[/]", ValidAge());
    if (eAgeStr == "-1") return true;

    string eAddress = GetStringInput("[yellow]Ünvanı:[/]");
    if (eAddress == "-1") return true;

    await employeeController.CreateAsync(new EmployeeCreateDto { Name = eName, Surname = eSurname, Age = int.Parse(eAgeStr), Address = eAddress, DepartmentId = eDeptId });
    return false;
}

static async Task<bool> HandleUpdateEmployee(EmployeeController employeeController, IDepartmentService departmentService, IEmployeeService employeeService)
{
    var allEmps = await employeeService.GetAll();
    if (!EnsureAny(allEmps.Any(), "Sistemdə heç bir işçi yoxdur!")) return false;
    await employeeController.PrintAllAsync();
    AnsiConsole.MarkupLine("");
    string updateEIdStr = GetStringInput("[yellow]Yenilənəcək Employee ID:[/]", IdExistsValidator(id => employeeService.GetById(id).GetAwaiter().GetResult()!));
    if (updateEIdStr == "-1") return true;
    int updateEId = int.Parse(updateEIdStr);

    AnsiConsole.MarkupLine("[grey]Diqqət: Boş buraxsanız köhnə dəyər saxlanılacaq.[/]");

    string newEName = GetStringInput("[yellow]Yeni Employee adı:[/]", ValidName("Ad"), allowEmpty: true);
    if (newEName == "-1") return true;

    string newESurname = GetStringInput("[yellow]Yeni Employee soyadı:[/]", ValidName("Soyad"), allowEmpty: true);
    if (newESurname == "-1") return true;

    string ageInput = GetStringInput("[yellow]Yeni Yaşı:[/]", ValidAge(), allowEmpty: true);
    if (ageInput == "-1") return true;

    string newEAddress = GetStringInput("[yellow]Yeni Ünvanı:[/]", null, allowEmpty: true);
    if (newEAddress == "-1") return true;

    string deptInput = GetStringInput("[yellow]Yeni Department ID:[/]", IdExistsValidator(id => departmentService.GetById(id).GetAwaiter().GetResult()!), allowEmpty: true);
    if (deptInput == "-1") return true;

    await employeeController.UpdateAsync(new EmployeeUpdateDto
    {
        Id = updateEId,
        Name = string.IsNullOrEmpty(newEName) ? null : newEName,
        Surname = string.IsNullOrEmpty(newESurname) ? null : newESurname,
        Age = string.IsNullOrEmpty(ageInput) ? 0 : int.Parse(ageInput),
        Address = string.IsNullOrEmpty(newEAddress) ? null : newEAddress,
        DepartmentId = string.IsNullOrEmpty(deptInput) ? 0 : int.Parse(deptInput)
    });
    return false;
}

static async Task<bool> HandleGetEmployeeById(EmployeeController employeeController, IEmployeeService employeeService)
{
    var allEmps = await employeeService.GetAll();
    return await HandleGetById(
        allEmps.Any(),
        "Sistemdə heç bir işçi yoxdur!",
        "Employee ID:",
        id => employeeService.GetById(id).GetAwaiter().GetResult()!,
        async id => { await employeeController.PrintByIdAsync(id); });
}

static async Task<bool> HandleDeleteEmployee(EmployeeController employeeController, IEmployeeService employeeService)
{
    var allEmps = await employeeService.GetAll();
    return await HandleDelete(
        allEmps.Any(),
        "Sistemdə heç bir işçi yoxdur!",
        async () => { await employeeController.PrintAllAsync(); },
        "Silinəcək Employee ID:",
        id => employeeService.GetById(id).GetAwaiter().GetResult()!,
        "Bu işçini silmək istədiyinizə əminsiniz?",
        async id => { await employeeController.DeleteAsync(id); });
}

static async Task<bool> HandleGetEmployeesByAge(EmployeeController employeeController)
{
    string searchAgeStr = GetStringInput("[yellow]Yaş:[/]", ValidAge());
    if (searchAgeStr == "-1") return true;
    await employeeController.PrintByAgeAsync(int.Parse(searchAgeStr));
    return false;
}

static async Task<bool> HandleGetEmployeesByDepartmentId(EmployeeController employeeController, DepartmentController departmentController, IDepartmentService departmentService)
{
    var allDepts = await departmentService.GetAll();
    if (!EnsureAny(allDepts.Any(), "Sistemdə heç bir departament yoxdur!")) return false;
    await departmentController.PrintAllAsync();
    AnsiConsole.MarkupLine("");
    string searchDeptIdStr = GetStringInput("[yellow]Department ID:[/]", IdExistsValidator(id => departmentService.GetById(id).GetAwaiter().GetResult()!));
    if (searchDeptIdStr == "-1") return true;
    await employeeController.PrintByDepartmentIdAsync(int.Parse(searchDeptIdStr));
    return false;
}

static async Task<bool> HandleGetAllEmployeesByDepartmentName(EmployeeController employeeController, DepartmentController departmentController, IDepartmentService departmentService)
{
    var allDepts = await departmentService.GetAll();
    if (!EnsureAny(allDepts.Any(), "Sistemdə heç bir departament yoxdur!")) return false;
    await departmentController.PrintAllAsync();
    AnsiConsole.MarkupLine("");
    string searchDeptName = GetStringInput("[yellow]Department Adı:[/]");
    if (searchDeptName == "-1") return true;
    await employeeController.PrintAllByDepartmentNameAsync(searchDeptName);
    return false;
}

static async Task HandleSearchEmployees(EmployeeController employeeController)
{
    while (true)
    {
        string searchE = GetStringInput("[yellow]Axtarış mətni (boş qoysanız hamısı gələcək):[/]", null, true);
        if (searchE == "-1") break;

        bool found = await employeeController.SearchAsync(searchE);
        if (found) break;
    }
}

static string GetStringInput(string message, Func<string, ValidationResult>? validator = null, bool allowEmpty = false)
{
    while (true)
    {
        AnsiConsole.Markup(message + " ");
        var sb = new StringBuilder();
        bool cancelled = false;

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                cancelled = true;
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (sb.Length > 0)
                {
                    sb.Length--;
                    Console.Write("\b \b");
                }
            }
            else if (key.KeyChar >= 32)
            {
                sb.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }

        if (cancelled) return "-1";

        string val = sb.ToString().Trim();

        if (string.IsNullOrEmpty(val) && !allowEmpty)
        {
            AnsiConsole.MarkupLine("[red]Boş buraxıla bilməz[/]");
            continue;
        }

        if (string.IsNullOrEmpty(val) && allowEmpty) return val;

        if (validator != null)
        {
            var result = validator(val);
            if (!result.Successful)
            {
                AnsiConsole.MarkupLine(result.Message ?? "[red]Xəta[/]");
                continue;
            }
        }

        return val;
    }
}

static IRenderable GetPreviewForOption(MenuOption option, Dictionary<MenuOption, (string Title, string Desc)> texts, IDepartmentService deptService, IEmployeeService empService)
{
    var content = new StringBuilder();
    if (texts.TryGetValue(option, out var text))
    {
        string color = option == MenuOption.Exit ? "red" : "cyan";
        content.AppendLine($"[bold {color}]{text.Title}[/]\n");
        content.AppendLine(text.Desc);
    }

    if (option == MenuOption.CreateDepartment)
        content.AppendLine($"Hazırkı departament sayı: [green]{deptService.GetAll().GetAwaiter().GetResult().Count()}[/]");
    else if (option == MenuOption.CreateEmployee || option == MenuOption.GetAllEmployeesCount)
        content.AppendLine($"\nHazırkı işçi sayı: [green]{empService.GetAll().GetAwaiter().GetResult().Count()}[/]");

    return PadToLines(content, 11);
}

static IRenderable GetStatsPanelContent(IDepartmentService deptService, IEmployeeService empService)
{
    var content = new StringBuilder();

    var depts = deptService.GetAll().GetAwaiter().GetResult().ToList();
    var emps = empService.GetAll().GetAwaiter().GetResult().ToList();

    content.AppendLine($"Ümumi Departament Sayı: [bold]{depts.Count}[/]");
    content.AppendLine($"Ümumi İşçi Sayı: [bold]{emps.Count}[/]");

    if (emps.Count > 0)
    {
        double avgAge = emps.Average(e => e.Age);
        content.AppendLine($"Orta Yaş Göstəricisi: [bold]{avgAge:F1}[/]");
    }
    else
    {
        content.AppendLine("Orta Yaş Göstəricisi: [bold]0[/]");
    }

    if (depts.Count > 0)
    {
        var biggestDept = depts.OrderByDescending(d => empService.GetByDepartmentIdAsync(d.Id).GetAwaiter().GetResult().Count()).FirstOrDefault();
        if (biggestDept != null)
            content.AppendLine($"Ən Böyük Departament: [bold]{biggestDept!.Name}[/]");
        else
            content.AppendLine("Ən Böyük Departament: [bold]Yoxdur[/]");

        int totalCapacity = depts.Sum(d => d.Capacity);
        int totalEmps = emps.Count;
        int vacancies = totalCapacity - totalEmps;
        content.AppendLine($"Boş İş Yeri Tutumu: [bold]{vacancies}[/]");
    }
    else
    {
        content.AppendLine("Ən Böyük Departament: [bold]Yoxdur[/]");
        content.AppendLine("Boş İş Yeri Tutumu: [bold]0[/]");
    }

    return PadToLines(content, 8);
}