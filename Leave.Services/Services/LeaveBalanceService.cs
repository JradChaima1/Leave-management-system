using Leave.Core.Models;
using Leave.Core.Interfaces;
using Leave.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace Leave.Services.Services
{
    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly IRepository<LeaveBalance> _leaveBalanceRepository;
        private readonly ILogger<LeaveBalanceService> _logger;

        public LeaveBalanceService(IRepository<LeaveBalance> leaveBalanceRepository, ILogger<LeaveBalanceService> logger)
        {
            _leaveBalanceRepository = leaveBalanceRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<LeaveBalance>> GetEmployeeBalancesAsync(int employeeId)
        {
            _logger.LogInformation("Fetching leave balances for employee {EmployeeId}", employeeId);
            var balances = await _leaveBalanceRepository.GetAllAsync();
            var result = balances.Where(b => b.EmployeeId == employeeId);
            _logger.LogInformation("Retrieved {Count} leave balances for employee {EmployeeId}", result.Count(), employeeId);
            return result;
        }

        public async Task<LeaveBalance> GetBalanceByTypeAsync(int employeeId, int leaveTypeId, int year)
        {
            _logger.LogInformation("Fetching leave balance for employee {EmployeeId}, type {LeaveTypeId}, year {Year}", 
                employeeId, leaveTypeId, year);
            var balances = await _leaveBalanceRepository.GetAllAsync();
            return balances.FirstOrDefault(b => 
                b.EmployeeId == employeeId && 
                b.LeaveTypeId == leaveTypeId && 
                b.Year == year);
        }

        public async Task UpdateBalanceAsync(LeaveBalance balance)
        {
            _logger.LogInformation("Updating leave balance for employee {EmployeeId}", balance.EmployeeId);
            await _leaveBalanceRepository.UpdateAsync(balance);
            _logger.LogInformation("Leave balance updated successfully for employee {EmployeeId}", balance.EmployeeId);
        }

        public async Task DeductLeaveAsync(int employeeId, int leaveTypeId, decimal days)
        {
            _logger.LogInformation("Deducting {Days} days from employee {EmployeeId}, leave type {LeaveTypeId}", 
                days, employeeId, leaveTypeId);
            
            var balance = await GetBalanceByTypeAsync(employeeId, leaveTypeId, DateTime.Now.Year);
            if (balance == null)
            {
                _logger.LogWarning("Deduction failed: Leave balance not found for employee {EmployeeId}", employeeId);
                throw new NotFoundException("Leave balance not found");
            }

            if (balance.RemainingDays < days)
            {
                _logger.LogWarning("Deduction failed: Insufficient balance for employee {EmployeeId}. Remaining: {RemainingDays}, Requested: {Days}", 
                    employeeId, balance.RemainingDays, days);
                throw new ValidationException("Insufficient leave balance");
            }

            balance.UsedDays += days;
            balance.RemainingDays -= days;

            await _leaveBalanceRepository.UpdateAsync(balance);
            _logger.LogInformation("Leave deducted successfully: Employee {EmployeeId}, Used {UsedDays}, Remaining {RemainingDays}", 
                employeeId, balance.UsedDays, balance.RemainingDays);
        }

        public async Task ResetAnnualBalancesAsync(int year)
        {
            _logger.LogInformation("Resetting annual leave balances for year {Year}", year);
            
            var balances = await _leaveBalanceRepository.GetAllAsync();
            var currentYearBalances = balances.Where(b => b.Year == year);

            int count = 0;
            foreach (var balance in currentYearBalances)
            {
                balance.UsedDays = 0;
                balance.RemainingDays = balance.TotalDays;
                await _leaveBalanceRepository.UpdateAsync(balance);
                count++;
            }
            
            _logger.LogInformation("Reset {Count} leave balances for year {Year}", count, year);
        }

        public async Task<decimal> GetRemainingBalanceAsync(int employeeId, int leaveTypeId)
        {
            var balance = await GetBalanceByTypeAsync(employeeId, leaveTypeId, DateTime.Now.Year);
            return balance?.RemainingDays ?? 0;
        }
    }
}
