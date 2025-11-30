using Microsoft.EntityFrameworkCore;
using Leave.Core.Models;
using Leave.Core.Interfaces;

namespace Leave.Data.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(LeaveContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByManagerAsync(int managerId)
        {
            return await _dbSet
                .Where(e => e.ManagerId == managerId && e.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            return await _dbSet
                .Where(e => e.Department == department && e.IsActive)
                .ToListAsync();
        }

        public async Task<Employee> GetEmployeeWithLeaveRequestsAsync(int employeeId)
        {
            return await _dbSet
                .Include(e => e.LeaveRequests)
                .ThenInclude(lr => lr.LeaveType)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<Employee> GetEmployeeWithBalancesAsync(int employeeId)
        {
            return await _dbSet
                .Include(e => e.LeaveBalances)
                .ThenInclude(lb => lb.LeaveType)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }
    }
}
