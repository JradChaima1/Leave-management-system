using Leave.Core.Models;

namespace Leave.Core.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        Task<IEnumerable<Employee>> GetEmployeesByManagerAsync(int managerId);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department);
        Task<Employee> GetEmployeeWithDetailsAsync(int employeeId);
    }
}
