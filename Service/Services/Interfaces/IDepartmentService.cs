using Service.DTOs;
using Service.Enums;

namespace Service.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task CreateAsync(DepartmentCreateDto dto);
        Task UpdateAsync(DepartmentUpdateDto dto);
        Task DeleteAsync(int id);
        Task<DepartmentDto> GetByIdAsync(int id);
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
        Task<IEnumerable<DepartmentDto>> SearchAsync(string text);
        Task<IEnumerable<DepartmentDto>> SortByCapacityAsync(SortDirection direction);
    }
}
