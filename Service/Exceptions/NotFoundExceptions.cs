namespace Service.Exceptions
{
    public class DepartmentNotFoundException : Exception
    {
        public DepartmentNotFoundException(string message) : base(message)
        {
        }
    }

    public class EmployeeNotFoundException : Exception
    {
        public EmployeeNotFoundException(string message) : base(message)
        {
        }
    }
}
