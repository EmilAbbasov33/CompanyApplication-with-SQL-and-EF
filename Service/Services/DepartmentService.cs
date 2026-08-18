using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Enums;
using Service.Exceptions;
using Service.Helpers.Constants;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public DepartmentService(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
        }

        public void Create(Department department)
        {
            if (department == null)
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (string.IsNullOrWhiteSpace(department.Name))
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (department.Capacity < 0)
                throw new ValidationException(ValidationConstants.NegativeCapacityMessage);

            if (_departmentRepository.GetAll().Any(d => d.Name.Equals(department.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ValidationException(ValidationConstants.DepartmentNameExistsMessage);

            _departmentRepository.Create(department);
        }

        public void Update(Department department)
        {
            if (department == null)
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            var existingDepartment = _departmentRepository.GetById(department.Id);
            if (existingDepartment == null)
                throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

            if (!string.IsNullOrWhiteSpace(department.Name))
            {
                if (!existingDepartment.Name.Equals(department.Name, StringComparison.OrdinalIgnoreCase) &&
                    _departmentRepository.GetAll().Any(d => d.Name.Equals(department.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ValidationException(ValidationConstants.DepartmentNameExistsMessage);
                }

                existingDepartment.Name = department.Name;
            }

            if (department.Capacity >= 0)
            {
                int currentEmployeeCount = _employeeRepository.GetByDepartmentId(department.Id).Count();
                if (department.Capacity < currentEmployeeCount)
                    throw new ValidationException(string.Format(ValidationConstants.CapacityTooSmallMessage, currentEmployeeCount));

                existingDepartment.Capacity = department.Capacity;
            }

            _departmentRepository.Update(existingDepartment);
        }

        public void Delete(int id)
        {
            var existingDepartment = _departmentRepository.GetById(id);
            if (existingDepartment == null)
                throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

            var employees = _employeeRepository.GetByDepartmentId(id);
            foreach (var emp in employees)
            {
                _employeeRepository.Delete(emp.Id);
            }

            _departmentRepository.Delete(id);
        }

        public Department GetById(int id)
        {
            var existingDepartment = _departmentRepository.GetById(id);
            if (existingDepartment == null)
                throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

            return existingDepartment;
        }

        public IEnumerable<Department> GetAll()
        {
            return _departmentRepository.GetAll();
        }

        public IEnumerable<Department> Search(string text)
        {
            return _departmentRepository.Search(text);
        }

        public IEnumerable<Department> SortByCapacity(SortDirection direction)
        {
            var departments = _departmentRepository.GetAll();

            if (direction == SortDirection.Ascending)
            {
                return departments.OrderBy(d => d.Capacity);
            }
            else
            {
                return departments.OrderByDescending(d => d.Capacity);
            }
        }
    }
}

