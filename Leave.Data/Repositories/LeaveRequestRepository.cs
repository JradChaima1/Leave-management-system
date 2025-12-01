using Microsoft.EntityFrameworkCore;
using Leave.Core.Models;
using Leave.Core.Interfaces;

namespace Leave.Data.Repositories
{
    public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(LeaveContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LeaveRequest>> GetRequestsByEmployeeAsync(int employeeId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(lr => lr.LeaveType)
                .Include(lr => lr.Approver)
                .Where(lr => lr.EmployeeId == employeeId)
                .OrderByDescending(lr => lr.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetPendingRequestsByManagerAsync(int managerId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(lr => lr.Employee)
                .Include(lr => lr.LeaveType)
                .Where(lr => lr.Employee.ManagerId == managerId && lr.Status == "Pending")
                .OrderBy(lr => lr.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetRequestsByStatusAsync(string status)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(lr => lr.Employee)
                .Include(lr => lr.LeaveType)
                .Where(lr => lr.Status == status)
                .OrderByDescending(lr => lr.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetRequestsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(lr => lr.Employee)
                .Include(lr => lr.LeaveType)
                .Where(lr => lr.StartDate >= startDate && lr.EndDate <= endDate)
                .OrderBy(lr => lr.StartDate)
                .ToListAsync();
        }
    }
}
