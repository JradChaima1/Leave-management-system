using Leave.Core.Models;
using Leave.Core.Interfaces;

namespace Leave.Services.Services
{
    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly IRepository<LeaveBalance> _leaveBalanceRepository;

        public LeaveBalanceService(IRepository<LeaveBalance> leaveBalanceRepository)
        {
            _leaveBalanceRepository = leaveBalanceRepository;
        }

        public async Task<IEnumerable<LeaveBalance>> GetEmployeeBalancesAsync(int employeeId)
        {
            var balances = await _leaveBalanceRepository.GetAllAsync();
            return balances.Where(b => b.EmployeeId == employeeId);
        }

        public async Task<LeaveBalance> GetBalanceByTypeAsync(int employeeId, int leaveTypeId, int year)
        {
            var balances = await _leaveBalanceRepository.GetAllAsync();
            return balances.FirstOrDefault(b => 
                b.EmployeeId == employeeId && 
                b.LeaveTypeId == leaveTypeId && 
                b.Year == year);
        }

        public async Task UpdateBalanceAsync(LeaveBalance balance)
        {
            await _leaveBalanceRepository.UpdateAsync(balance);
        }

        public async Task DeductLeaveAsync(int employeeId, int leaveTypeId, decimal days)
        {
            var balance = await GetBalanceByTypeAsync(employeeId, leaveTypeId, DateTime.Now.Year);
            if (balance == null)
                throw new Exception("Leave balance not found");

            if (balance.RemainingDays < days)
                throw new Exception("Insufficient leave balance");

            balance.UsedDays += days;
            balance.RemainingDays -= days;

            await _leaveBalanceRepository.UpdateAsync(balance);
        }

        public async Task ResetAnnualBalancesAsync(int year)
        {
            var balances = await _leaveBalanceRepository.GetAllAsync();
            var currentYearBalances = balances.Where(b => b.Year == year);

            foreach (var balance in currentYearBalances)
            {
                balance.UsedDays = 0;
                balance.RemainingDays = balance.TotalDays;
                await _leaveBalanceRepository.UpdateAsync(balance);
            }
        }

        public async Task<decimal> GetRemainingBalanceAsync(int employeeId, int leaveTypeId)
        {
            var balance = await GetBalanceByTypeAsync(employeeId, leaveTypeId, DateTime.Now.Year);
            return balance?.RemainingDays ?? 0;
        }
    }
}
