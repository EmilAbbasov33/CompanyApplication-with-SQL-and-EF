namespace Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public int Age { get; set; }
        public string Address { get; set; } = null!;
        public int DepartmentId { get; set; }

        public Department Department { get; set; } = null!;
    }
}
