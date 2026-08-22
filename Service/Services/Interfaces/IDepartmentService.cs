using Service.DTOs;
using Service.Enums;

namespace Service.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> SearchAsync(string text);
        Task<IEnumerable<DepartmentDto>> SortByCapacityAsync(SortDirection direction);
    }
}
