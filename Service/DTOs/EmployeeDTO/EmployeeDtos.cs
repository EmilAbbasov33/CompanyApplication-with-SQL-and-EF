using Service.DTOs.DepartmentDTO;

namespace Service.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public int Age { get; set; }
        public string Address { get; set; } = null!;
        public int DepartmentId { get; set; }
        public DepartmentDto Department { get; set; } = null!;
    }
}
