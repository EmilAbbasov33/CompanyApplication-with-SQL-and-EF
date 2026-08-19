using AutoMapper;
using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.DTOs;
using Service.Enums;
using Service.Exceptions;
using Service.Helpers.Constants;
using Service.Profiles;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public DepartmentService(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
            
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingProfile());
            }, Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();
        }

        public async Task CreateAsync(DepartmentCreateDto dto)
        {
            if (dto == null)
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (dto.Capacity < 0)
                throw new ValidationException(ValidationConstants.NegativeCapacityMessage);

            var allDepartments = await _departmentRepository.GetAllAsync();
            if (allDepartments.Any(d => d.Name != null && d.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ValidationException(ValidationConstants.DepartmentNameExistsMessage);

            var department = _mapper.Map<Department>(dto);
            await _departmentRepository.CreateAsync(department);
        }

        public async Task UpdateAsync(DepartmentUpdateDto dto)
        {
            if (dto == null)
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            var existingDepartment = await _departmentRepository.GetByIdAsync(dto.Id);
            if (existingDepartment == null)
                throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                var allDepartments = await _departmentRepository.GetAllAsync();
                if (!existingDepartment.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) &&
                    allDepartments.Any(d => d.Name != null && d.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ValidationException(ValidationConstants.DepartmentNameExistsMessage);
                }

                existingDepartment.Name = dto.Name;
            }

            if (dto.Capacity.HasValue)
            {
                var employees = await _employeeRepository.GetByDepartmentIdAsync(dto.Id);
                int currentEmployeeCount = employees.Count();
                if (dto.Capacity.Value < currentEmployeeCount)
                    throw new ValidationException(string.Format(ValidationConstants.CapacityTooSmallMessage, currentEmployeeCount));

                existingDepartment.Capacity = dto.Capacity.Value;
            }

            await _departmentRepository.UpdateAsync(existingDepartment);
        }

        public async Task DeleteAsync(int id)
        {
            var existingDepartment = await _departmentRepository.GetByIdAsync(id);
            if (existingDepartment == null)
                throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

            var employees = await _employeeRepository.GetByDepartmentIdAsync(id);
            foreach (var emp in employees)
            {
                await _employeeRepository.DeleteAsync(emp.Id);
            }

            await _departmentRepository.DeleteAsync(id);
        }

        public async Task<DepartmentDto> GetByIdAsync(int id)
        {
            var existingDepartment = await _departmentRepository.GetByIdAsync(id);
            if (existingDepartment == null)
                throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

            return _mapper.Map<DepartmentDto>(existingDepartment);
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<IEnumerable<DepartmentDto>> SearchAsync(string text)
        {
            var departments = await _departmentRepository.SearchAsync(text);
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<IEnumerable<DepartmentDto>> SortByCapacityAsync(SortDirection direction)
        {
            var departments = await _departmentRepository.GetAllAsync();

            if (direction == SortDirection.Ascending)
            {
                departments = departments.OrderBy(d => d.Capacity).ToList();
            }
            else
            {
                departments = departments.OrderByDescending(d => d.Capacity).ToList();
            }

            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }
    }
}
