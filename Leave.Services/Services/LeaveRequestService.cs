using Leave.Core.Models;
using Leave.Core.Interfaces;

namespace Leave.Services.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRepository<LeaveBalance> _leaveBalanceRepository;
        private readonly IRepository<Holiday> _holidayRepository;

        public LeaveRequestService(
            ILeaveRequestRepository leaveRequestRepository,
            IEmployeeRepository employeeRepository,
            IRepository<LeaveBalance> leaveBalanceRepository,
            IRepository<Holiday> holidayRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeeRepository = employeeRepository;
            _leaveBalanceRepository = leaveBalanceRepository;
            _holidayRepository = holidayRepository;
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllRequestsAsync()
        {
            return await _leaveRequestRepository.GetAllAsync();
        }

        public async Task<LeaveRequest> GetRequestByIdAsync(int id)
        {
            var request = await _leaveRequestRepository.GetByIdAsync(id);
            if (request == null)
                throw new Exception($"Leave request with ID {id} not found");
            return request;
        }

        public async Task<LeaveRequest> SubmitLeaveRequestAsync(LeaveRequest request)
        {
         
            if (request.StartDate < DateTime.Now.Date)
                throw new Exception("Start date cannot be in the past");
            if (request.EndDate < request.StartDate)
                throw new Exception("End date must be after start date");

          
            request.TotalDays = await CalculateLeaveDaysAsync(request.StartDate, request.EndDate);

            
            if (!await ValidateLeaveRequestAsync(request))
                throw new Exception("Insufficient leave balance");

          
            request.Status = "Pending";
            request.RequestDate = DateTime.Now;

            return await _leaveRequestRepository.AddAsync(request);
        }

        public async Task ApproveRequestAsync(int requestId, int approvedBy)
        {
            var request = await _leaveRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new Exception($"Leave request with ID {requestId} not found");

            if (request.Status != "Pending")
                throw new Exception("Only pending requests can be approved");

            request.Status = "Approved";
            request.ApprovedBy = approvedBy;
            request.ApprovalDate = DateTime.Now;

            await _leaveRequestRepository.UpdateAsync(request);

  
            var balances = await _leaveBalanceRepository.GetAllAsync();
            var balance = balances.FirstOrDefault(b => 
                b.EmployeeId == request.EmployeeId && 
                b.LeaveTypeId == request.LeaveTypeId && 
                b.Year == DateTime.Now.Year);

            if (balance != null)
            {
                balance.UsedDays += request.TotalDays;
                balance.RemainingDays -= request.TotalDays;
                await _leaveBalanceRepository.UpdateAsync(balance);
            }
        }

        public async Task RejectRequestAsync(int requestId, int rejectedBy, string reason)
        {
            var request = await _leaveRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new Exception($"Leave request with ID {requestId} not found");

            if (request.Status != "Pending")
                throw new Exception("Only pending requests can be rejected");

            request.Status = "Rejected";
            request.ApprovedBy = rejectedBy;
            request.ApprovalDate = DateTime.Now;
            request.RejectionReason = reason;

            await _leaveRequestRepository.UpdateAsync(request);
        }

        public async Task CancelRequestAsync(int requestId)
        {
            var request = await _leaveRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new Exception($"Leave request with ID {requestId} not found");

            if (request.Status != "Pending")
                throw new Exception("Only pending requests can be cancelled");

            request.Status = "Cancelled";
            await _leaveRequestRepository.UpdateAsync(request);
        }

        public async Task<IEnumerable<LeaveRequest>> GetEmployeeRequestsAsync(int employeeId)
        {
            return await _leaveRequestRepository.GetRequestsByEmployeeAsync(employeeId);
        }

        public async Task<IEnumerable<LeaveRequest>> GetPendingRequestsForManagerAsync(int managerId)
        {
            return await _leaveRequestRepository.GetPendingRequestsByManagerAsync(managerId);
        }

        public async Task<bool> ValidateLeaveRequestAsync(LeaveRequest request)
        {
            var balances = await _leaveBalanceRepository.GetAllAsync();
            var balance = balances.FirstOrDefault(b => 
                b.EmployeeId == request.EmployeeId && 
                b.LeaveTypeId == request.LeaveTypeId && 
                b.Year == DateTime.Now.Year);

            if (balance == null)
                return false;

            return balance.RemainingDays >= request.TotalDays;
        }

        private async Task<decimal> CalculateLeaveDaysAsync(DateTime startDate, DateTime endDate)
        {
            decimal totalDays = 0;
            var holidays = await _holidayRepository.GetAllAsync();
            var holidayDates = holidays.Select(h => h.Date.Date).ToList();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
               
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                if (holidayDates.Contains(date))
                    continue;

                totalDays++;
            }

            return totalDays;
        }
    }
}
