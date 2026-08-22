namespace Service.DTOs.EmployeeDTO
{
    public class EmployeeUpdateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public int Age { get; set; }
        public string? Address { get; set; }
        public int DepartmentId { get; set; }
    }
}
