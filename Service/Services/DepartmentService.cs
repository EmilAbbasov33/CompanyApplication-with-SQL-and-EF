using AutoMapper;
using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.DTOs;
using Service.Enums;
using Service.Exceptions;
using Service.Helpers.Constants;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class DepartmentService
        : CrudManager<Department, DepartmentDto, DepartmentCreateDto, DepartmentUpdateDto>,
          IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public DepartmentService(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository, IMapper mapper) : base(departmentRepository, mapper)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
        }

        public override async Task Create(DepartmentCreateDto dto)
        {
            if (dto == null)
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (dto.Capacity < 0)
                throw new ValidationException(ValidationConstants.NegativeCapacityMessage);

            var allDepartments = await Repository.GetAllAsync();
            if (allDepartments.Any(d => d.Name != null && d.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ValidationException(ValidationConstants.DepartmentNameExistsMessage);

            await base.Create(dto);
        }

        public override async Task Update(DepartmentUpdateDto dto)
        {
            if (dto == null)
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            var existingDepartment = await Repository.GetByIdAsync(dto.Id);
            if (existingDepartment == null)
                throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                var allDepartments = await Repository.GetAllAsync();
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

            await Repository.UpdateAsync(existingDepartment);
        }

        public override async Task Remove(int id)
        {
            var existingDepartment = await Repository.GetByIdAsync(id);
            if (existingDepartment == null)
                throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

            var employees = await _employeeRepository.GetByDepartmentIdAsync(id);
            foreach (var emp in employees)
            {
                await _employeeRepository.DeleteAsync(emp.Id);
            }

            await Repository.DeleteAsync(id);
        }

        public override async Task<DepartmentDto?> GetById(int id)
        {
            var existingDepartment = await Repository.GetByIdAsync(id);
            if (existingDepartment == null)
                throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

            return Mapper.Map<DepartmentDto>(existingDepartment);
        }

        public async Task<IEnumerable<DepartmentDto>> SearchAsync(string text)
        {
            var departments = await _departmentRepository.SearchAsync(text);
            return Mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<IEnumerable<DepartmentDto>> SortByCapacityAsync(SortDirection direction)
        {
            var departments = await Repository.GetAllAsync();

            if (direction == SortDirection.Ascending)
            {
                departments = departments.OrderBy(d => d.Capacity).ToList();
            }
            else
            {
                departments = departments.OrderByDescending(d => d.Capacity).ToList();
            }

            return Mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }
    }
}
