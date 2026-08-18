using Domain.Entities;
using Service.Enums;

namespace Service.Services.Interfaces
{
    public interface IDepartmentService
    {
        void Create(Department department);
        void Update(Department department);
        void Delete(int id);
        Department GetById(int id);
        IEnumerable<Department> GetAll();
        IEnumerable<Department> Search(string text);
        IEnumerable<Department> SortByCapacity(SortDirection direction);
    }
}
