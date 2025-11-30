using Leave.Core.Models;
using Leave.Core.Interfaces;

namespace Leave.Services.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _departmentRepository;

        public DepartmentService(IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                throw new Exception($"Department with ID {id} not found");
            return department;
        }

        public async Task<Department> CreateDepartmentAsync(Department department)
        {
            if (string.IsNullOrEmpty(department.Name))
                throw new Exception("Department name is required");

            return await _departmentRepository.AddAsync(department);
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            var existing = await _departmentRepository.GetByIdAsync(department.DepartmentId);
            if (existing == null)
                throw new Exception($"Department with ID {department.DepartmentId} not found");

            await _departmentRepository.UpdateAsync(department);
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                throw new Exception($"Department with ID {id} not found");

            await _departmentRepository.DeleteAsync(id);
        }

        public async Task<Department> GetDepartmentWithEmployeesAsync(int id)
        {
            return await _departmentRepository.GetByIdAsync(id);
        }
    }
}
