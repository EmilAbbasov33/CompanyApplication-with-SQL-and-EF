using AutoMapper;
using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Delegates;
using Service.DTOs;
using Service.Exceptions;
using Service.Helpers.Constants;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class EmployeeService
        : CrudManager<Employee, EmployeeDto, EmployeeCreateDto, EmployeeUpdateDto>,
          IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public EmployeeService(IEmployeeRepository employeeRepository,
                               IDepartmentRepository departmentRepository,
                               IMapper mapper)
            : base(employeeRepository, mapper)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        private async Task<EmployeeDto> PopulateDepartmentAsync(Employee employee)
        {
            var dto = Mapper.Map<EmployeeDto>(employee);
            var department = await _departmentRepository.GetByIdAsync(employee.DepartmentId);
            if (department != null)
            {
                dto.Department = Mapper.Map<DepartmentDto>(department);
            }
            return dto;
        }

        public override async Task Create(EmployeeCreateDto dto)
        {
            if (dto == null)
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            ValidationRule<string> nameRule = NameValidator.IsValidName;

            if (!nameRule(dto.Name))
                throw new ValidationException(ValidationConstants.InvalidNameMessage);

            if (string.IsNullOrWhiteSpace(dto.Surname))
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (!nameRule(dto.Surname))
                throw new ValidationException(ValidationConstants.InvalidNameMessage);

            if (string.IsNullOrWhiteSpace(dto.Address))
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (dto.Age < ValidationConstants.MinAge || dto.Age > ValidationConstants.MaxAge)
                throw new ValidationException(ValidationConstants.InvalidAgeMessage);

            var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
            if (department == null)
                throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

            if (department.Capacity == 0)
                throw new ValidationException(string.Format(ValidationConstants.DepartmentCapacityZeroMessage, department.Name));

            var employees = await _employeeRepository.GetByDepartmentIdAsync(dto.DepartmentId);
            int currentEmployeeCount = employees.Count();
            if (currentEmployeeCount >= department.Capacity)
                throw new ValidationException(string.Format(ValidationConstants.DepartmentFullMessage, department.Name, department.Capacity));

            await base.Create(dto);
        }

        public override async Task Update(EmployeeUpdateDto dto)
        {
            if (dto == null)
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            var existingEmployee = await Repository.GetByIdAsync(dto.Id);
            if (existingEmployee == null)
                throw new EmployeeNotFoundException(ValidationConstants.EmployeeNotFoundMessage);

            ValidationRule<string> nameRule = NameValidator.IsValidName;

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (!nameRule(dto.Name))
                    throw new ValidationException(ValidationConstants.InvalidNameMessage);
                existingEmployee.Name = dto.Name;
            }

            if (!string.IsNullOrWhiteSpace(dto.Surname))
            {
                if (!nameRule(dto.Surname))
                    throw new ValidationException(ValidationConstants.InvalidNameMessage);
                existingEmployee.Surname = dto.Surname;
            }

            if (dto.Age > 0)
            {
                if (dto.Age < ValidationConstants.MinAge || dto.Age > ValidationConstants.MaxAge)
                    throw new ValidationException(ValidationConstants.InvalidAgeMessage);
                existingEmployee.Age = dto.Age;
            }

            if (!string.IsNullOrWhiteSpace(dto.Address))
            {
                existingEmployee.Address = dto.Address;
            }

            if (dto.DepartmentId > 0)
            {
                var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
                if (department == null)
                    throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

                if (dto.DepartmentId != existingEmployee.DepartmentId)
                {
                    if (department.Capacity == 0)
                        throw new ValidationException(string.Format(ValidationConstants.DepartmentCapacityZeroMessage, department.Name));

                    var employees = await _employeeRepository.GetByDepartmentIdAsync(dto.DepartmentId);
                    int currentEmployeeCount = employees.Count();
                    if (currentEmployeeCount >= department.Capacity)
                        throw new ValidationException(string.Format(ValidationConstants.DepartmentFullMessage, department.Name, department.Capacity));
                }

                existingEmployee.DepartmentId = dto.DepartmentId;
            }

            await Repository.UpdateAsync(existingEmployee);
        }

        public override async Task Remove(int id)
        {
            var existingEmployee = await Repository.GetByIdAsync(id);
            if (existingEmployee == null)
                throw new EmployeeNotFoundException(ValidationConstants.EmployeeNotFoundMessage);

            await Repository.DeleteAsync(id);
        }

        public override async Task<IEnumerable<EmployeeDto>> GetAll()
        {
            var employees = await Repository.GetAllAsync();
            var dtos = new List<EmployeeDto>();
            foreach (var emp in employees)
            {
                dtos.Add(await PopulateDepartmentAsync(emp));
            }
            return dtos;
        }

        public override async Task<EmployeeDto?> GetById(int id)
        {
            var existingEmployee = await Repository.GetByIdAsync(id);
            if (existingEmployee == null)
                throw new EmployeeNotFoundException(ValidationConstants.EmployeeNotFoundMessage);

            return await PopulateDepartmentAsync(existingEmployee);
        }

        public async Task<IEnumerable<EmployeeDto>> GetByAgeAsync(int age)
        {
            var employees = await _employeeRepository.GetByAgeAsync(age);
            var dtos = new List<EmployeeDto>();
            foreach (var emp in employees)
            {
                dtos.Add(await PopulateDepartmentAsync(emp));
            }
            return dtos;
        }

        public async Task<IEnumerable<EmployeeDto>> GetByDepartmentIdAsync(int departmentId)
        {
            var employees = await _employeeRepository.GetByDepartmentIdAsync(departmentId);
            var dtos = new List<EmployeeDto>();
            foreach (var emp in employees)
            {
                dtos.Add(await PopulateDepartmentAsync(emp));
            }
            return dtos;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllByDepartmentNameAsync(string departmentName)
        {
            var departments = await _departmentRepository.SearchAsync(departmentName);
            var departmentIds = departments.Select(d => d.Id).ToList();

            var allEmployees = await Repository.GetAllAsync();
            var employees = allEmployees.Where(e => departmentIds.Contains(e.DepartmentId));

            var dtos = new List<EmployeeDto>();
            foreach (var emp in employees)
            {
                dtos.Add(await PopulateDepartmentAsync(emp));
            }
            return dtos;
        }

        public async Task<IEnumerable<EmployeeDto>> SearchAsync(string text)
        {
            var employees = await _employeeRepository.SearchAsync(text);
            var dtos = new List<EmployeeDto>();
            foreach (var emp in employees)
            {
                dtos.Add(await PopulateDepartmentAsync(emp));
            }
            return dtos;
        }

        public async Task<int> GetCountAsync()
        {
            var employees = await Repository.GetAllAsync();
            return employees.Count();
        }
    }
}
