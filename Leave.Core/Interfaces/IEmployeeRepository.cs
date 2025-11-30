using Leave.Core.Models;

namespace Leave.Core.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetEmployeesByManagerAsync(int managerId);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department);
        Task<Employee> GetEmployeeWithLeaveRequestsAsync(int employeeId);
        Task<Employee> GetEmployeeWithBalancesAsync(int employeeId);
    }
}
