using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Delegates;
using Service.Exceptions;
using Service.Helpers.Constants;
using Service.Services.Interfaces;

namespace Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        private Employee PopulateDepartment(Employee employee)
        {
            employee.Department = _departmentRepository.GetById(employee.DepartmentId)!;
            return employee;
        }

        public void Create(Employee employee)
        {
            if (employee == null)
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (string.IsNullOrWhiteSpace(employee.Name))
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            ValidationRule<string> nameRule = NameValidator.IsValidName;

            if (!nameRule(employee.Name))
                throw new ValidationException(ValidationConstants.InvalidNameMessage);

            if (string.IsNullOrWhiteSpace(employee.Surname))
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (!nameRule(employee.Surname))
                throw new ValidationException(ValidationConstants.InvalidNameMessage);

            if (string.IsNullOrWhiteSpace(employee.Address))
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            if (employee.Age < ValidationConstants.MinAge || employee.Age > ValidationConstants.MaxAge)
                throw new ValidationException(ValidationConstants.InvalidAgeMessage);

            var department = _departmentRepository.GetById(employee.DepartmentId);
            if (department == null)
                throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

            if (department.Capacity == 0)
                throw new ValidationException(string.Format(ValidationConstants.DepartmentCapacityZeroMessage, department.Name));

            int currentEmployeeCount = _employeeRepository.GetByDepartmentId(employee.DepartmentId).Count();
            if (currentEmployeeCount >= department.Capacity)
                throw new ValidationException(string.Format(ValidationConstants.DepartmentFullMessage, department.Name, department.Capacity));

            _employeeRepository.Create(employee);
        }

        public void Update(Employee employee)
        {
            if (employee == null)
                throw new ValidationException(ValidationConstants.RequiredFieldMessage);

            var existingEmployee = _employeeRepository.GetById(employee.Id);
            if (existingEmployee == null)
                throw new EmployeeNotFoundException(ValidationConstants.EmployeeNotFoundMessage);

            ValidationRule<string> nameRule = NameValidator.IsValidName;

            if (!string.IsNullOrWhiteSpace(employee.Name))
            {
                if (!nameRule(employee.Name))
                    throw new ValidationException(ValidationConstants.InvalidNameMessage);
                existingEmployee.Name = employee.Name;
            }

            if (!string.IsNullOrWhiteSpace(employee.Surname))
            {
                if (!nameRule(employee.Surname))
                    throw new ValidationException(ValidationConstants.InvalidNameMessage);
                existingEmployee.Surname = employee.Surname;
            }

            if (employee.Age > 0)
            {
                if (employee.Age < ValidationConstants.MinAge || employee.Age > ValidationConstants.MaxAge)
                    throw new ValidationException(ValidationConstants.InvalidAgeMessage);
                existingEmployee.Age = employee.Age;
            }

            if (!string.IsNullOrWhiteSpace(employee.Address))
            {
                existingEmployee.Address = employee.Address;
            }

            if (employee.DepartmentId > 0)
            {
                var department = _departmentRepository.GetById(employee.DepartmentId);
                if (department == null)
                    throw new DepartmentNotFoundException(ValidationConstants.DepartmentNotFoundMessage);

                if (employee.DepartmentId != existingEmployee.DepartmentId)
                {
                    if (department.Capacity == 0)
                        throw new ValidationException(string.Format(ValidationConstants.DepartmentCapacityZeroMessage, department.Name));

                    int currentEmployeeCount = _employeeRepository.GetByDepartmentId(employee.DepartmentId).Count();
                    if (currentEmployeeCount >= department.Capacity)
                        throw new ValidationException(string.Format(ValidationConstants.DepartmentFullMessage, department.Name, department.Capacity));
                }

                existingEmployee.DepartmentId = employee.DepartmentId;
            }

            _employeeRepository.Update(existingEmployee);
        }

        public void Delete(int id)
        {
            var existingEmployee = _employeeRepository.GetById(id);
            if (existingEmployee == null)
                throw new EmployeeNotFoundException(ValidationConstants.EmployeeNotFoundMessage);

            _employeeRepository.Delete(id);
        }

        public Employee GetById(int id)
        {
            var existingEmployee = _employeeRepository.GetById(id);
            if (existingEmployee == null)
                throw new EmployeeNotFoundException(ValidationConstants.EmployeeNotFoundMessage);

            return PopulateDepartment(existingEmployee);
        }

        public IEnumerable<Employee> GetByAge(int age)
        {
            return _employeeRepository.GetByAge(age).Select(PopulateDepartment);
        }

        public IEnumerable<Employee> GetByDepartmentId(int departmentId)
        {
            return _employeeRepository.GetByDepartmentId(departmentId).Select(PopulateDepartment);
        }

        public IEnumerable<Employee> GetAllByDepartmentName(string departmentName)
        {
            var departments = _departmentRepository.Search(departmentName);
            var departmentIds = departments.Select(d => d.Id).ToList();

            return _employeeRepository.GetAll()
                .Where(e => departmentIds.Contains(e.DepartmentId))
                .Select(PopulateDepartment);
        }

        public IEnumerable<Employee> Search(string text)
        {
            return _employeeRepository.Search(text).Select(PopulateDepartment);
        }

        public int GetCount()
        {
            return _employeeRepository.GetAll().Count();
        }

        public IEnumerable<Employee> GetAll()
        {
            return _employeeRepository.GetAll().Select(PopulateDepartment);
        }
    }
}

