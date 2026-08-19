namespace Service.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
    }

    public class DepartmentCreateDto
    {
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
    }

    public class DepartmentUpdateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Capacity { get; set; }
    }
}
