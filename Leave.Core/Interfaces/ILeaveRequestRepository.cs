using Leave.Core.Models;

namespace Leave.Core.Interfaces
{
    public interface ILeaveRequestRepository : IRepository<LeaveRequest>
    {
        Task<IEnumerable<LeaveRequest>> GetRequestsByEmployeeAsync(int employeeId);
        Task<IEnumerable<LeaveRequest>> GetPendingRequestsByManagerAsync(int managerId);
        Task<IEnumerable<LeaveRequest>> GetRequestsByStatusAsync(string status);
        Task<IEnumerable<LeaveRequest>> GetRequestsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
