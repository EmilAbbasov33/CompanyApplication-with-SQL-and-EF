using AutoMapper;
using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Delegates;
using Service.DTOs;
using Service.Exceptions;
using Service.Helpers.Constants;
using Service.Profiles;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingProfile());
            }, Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();
        }

        private async Task<EmployeeDto> PopulateDepartmentAsync(Employee employee)
        {
            var dto = _mapper.Map<EmployeeDto>(employee);
            var department = await _departmentRepository.GetByIdAsync(employee.DepartmentId);
            if(department != null)
            {
                dto.Department = _mapper.Map<DepartmentDto>(department);
            }
            return dto;
        }

        public async Task CreateAsync(EmployeeCreateDto dto)
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

            var employee = _mapper.Map<Employee>(dto);
            await _employeeRepository.CreateAsync(employee);
        }

        public async Task UpdateAsync(EmployeeUpdateDto dto)
        {
            if (dto == null)
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            var existingEmployee = await _employeeRepository.GetByIdAsync(dto.Id);
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

            await _employeeRepository.UpdateAsync(existingEmployee);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEmployee = await _employeeRepository.GetByIdAsync(id);
            if (existingEmployee == null)
                throw new EmployeeNotFoundException(ValidationConstants.EmployeeNotFoundMessage);

            await _employeeRepository.DeleteAsync(id);
        }

        public async Task<EmployeeDto> GetByIdAsync(int id)
        {
            var existingEmployee = await _employeeRepository.GetByIdAsync(id);
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

            var allEmployees = await _employeeRepository.GetAllAsync();
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
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Count();
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            var dtos = new List<EmployeeDto>();
            foreach (var emp in employees)
            {
                dtos.Add(await PopulateDepartmentAsync(emp));
            }
            return dtos;
        }
    }
}
