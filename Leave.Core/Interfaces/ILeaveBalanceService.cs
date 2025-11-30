using Leave.Core.Models;

namespace Leave.Core.Interfaces
{
    public interface ILeaveBalanceService
    {
        Task<IEnumerable<LeaveBalance>> GetEmployeeBalancesAsync(int employeeId);
        Task<LeaveBalance> GetBalanceByTypeAsync(int employeeId, int leaveTypeId, int year);
        Task UpdateBalanceAsync(LeaveBalance balance);
        Task DeductLeaveAsync(int employeeId, int leaveTypeId, decimal days);
        Task ResetAnnualBalancesAsync(int year);
        Task<decimal> GetRemainingBalanceAsync(int employeeId, int leaveTypeId);
    }
}
