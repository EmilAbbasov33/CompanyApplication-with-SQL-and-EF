namespace Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }

    }
}
