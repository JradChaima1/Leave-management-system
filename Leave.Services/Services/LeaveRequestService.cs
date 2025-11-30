using Leave.Core.Models;
using Leave.Core.Interfaces;
using Leave.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace Leave.Services.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRepository<LeaveBalance> _leaveBalanceRepository;
        private readonly IRepository<Holiday> _holidayRepository;
        private readonly ILogger<LeaveRequestService> _logger;

        public LeaveRequestService(
            ILeaveRequestRepository leaveRequestRepository,
            IEmployeeRepository employeeRepository,
            IRepository<LeaveBalance> leaveBalanceRepository,
            IRepository<Holiday> holidayRepository,
            ILogger<LeaveRequestService> logger)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeeRepository = employeeRepository;
            _leaveBalanceRepository = leaveBalanceRepository;
            _holidayRepository = holidayRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllRequestsAsync()
        {
            _logger.LogInformation("Fetching all leave requests");
            var requests = await _leaveRequestRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} leave requests", requests.Count());
            return requests;
        }

        public async Task<LeaveRequest> GetRequestByIdAsync(int id)
        {
            _logger.LogInformation("Fetching leave request with ID: {RequestId}", id);
            var request = await _leaveRequestRepository.GetByIdAsync(id);
            
            if (request == null)
            {
                _logger.LogWarning("Leave request with ID {RequestId} not found", id);
                throw new NotFoundException($"Leave request with ID {id} not found");
            }
            
            return request;
        }

        public async Task<LeaveRequest> SubmitLeaveRequestAsync(LeaveRequest request)
        {
            _logger.LogInformation("Submitting leave request for employee {EmployeeId} from {StartDate} to {EndDate}", 
                request.EmployeeId, request.StartDate, request.EndDate);
            
            if (request.StartDate < DateTime.Now.Date)
            {
                _logger.LogWarning("Leave request rejected: Start date {StartDate} is in the past", request.StartDate);
                throw new ValidationException("Start date cannot be in the past");
            }
            if (request.EndDate < request.StartDate)
            {
                _logger.LogWarning("Leave request rejected: End date {EndDate} is before start date {StartDate}", 
                    request.EndDate, request.StartDate);
                throw new ValidationException("End date must be after start date");
            }

            request.TotalDays = await CalculateLeaveDaysAsync(request.StartDate, request.EndDate);
            _logger.LogInformation("Calculated {TotalDays} leave days (excluding weekends/holidays)", request.TotalDays);

            if (!await ValidateLeaveRequestAsync(request))
            {
                _logger.LogWarning("Leave request rejected: Insufficient leave balance for employee {EmployeeId}", 
                    request.EmployeeId);
                throw new ValidationException("Insufficient leave balance");
            }

            request.Status = "Pending";
            request.RequestDate = DateTime.Now;

            var result = await _leaveRequestRepository.AddAsync(request);
            _logger.LogInformation("Leave request submitted successfully: RequestId {RequestId}, Employee {EmployeeId}", 
                result.LeaveRequestId, result.EmployeeId);
            return result;
        }

        public async Task ApproveRequestAsync(int requestId, int approvedBy)
        {
            _logger.LogInformation("Approving leave request {RequestId} by user {ApprovedBy}", requestId, approvedBy);
            
            var request = await _leaveRequestRepository.GetByIdAsync(requestId);
            if (request == null)
            {
                _logger.LogWarning("Approval failed: Leave request {RequestId} not found", requestId);
                throw new NotFoundException($"Leave request with ID {requestId} not found");
            }

            if (request.Status != "Pending")
            {
                _logger.LogWarning("Approval failed: Request {RequestId} status is {Status}, not Pending", 
                    requestId, request.Status);
                throw new ValidationException("Only pending requests can be approved");
            }

            request.Status = "Approved";
            request.ApprovedBy = approvedBy;
            request.ApprovalDate = DateTime.Now;

            await _leaveRequestRepository.UpdateAsync(request);
            _logger.LogInformation("Leave request approved: RequestId {RequestId}, Employee {EmployeeId}, Days {TotalDays}", 
                request.LeaveRequestId, request.EmployeeId, request.TotalDays);

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
                _logger.LogInformation("Leave balance updated: Employee {EmployeeId}, Used {UsedDays}, Remaining {RemainingDays}", 
                    request.EmployeeId, balance.UsedDays, balance.RemainingDays);
            }
        }

        public async Task RejectRequestAsync(int requestId, int rejectedBy, string reason)
        {
            _logger.LogInformation("Rejecting leave request {RequestId} by user {RejectedBy}", requestId, rejectedBy);
            
            var request = await _leaveRequestRepository.GetByIdAsync(requestId);
            if (request == null)
            {
                _logger.LogWarning("Rejection failed: Leave request {RequestId} not found", requestId);
                throw new NotFoundException($"Leave request with ID {requestId} not found");
            }

            if (request.Status != "Pending")
            {
                _logger.LogWarning("Rejection failed: Request {RequestId} status is {Status}, not Pending", 
                    requestId, request.Status);
                throw new ValidationException("Only pending requests can be rejected");
            }

            request.Status = "Rejected";
            request.ApprovedBy = rejectedBy;
            request.ApprovalDate = DateTime.Now;
            request.RejectionReason = reason;

            await _leaveRequestRepository.UpdateAsync(request);
            _logger.LogInformation("Leave request rejected: RequestId {RequestId}, Employee {EmployeeId}, Reason: {Reason}", 
                request.LeaveRequestId, request.EmployeeId, reason);
        }

        public async Task CancelRequestAsync(int requestId)
        {
            _logger.LogInformation("Cancelling leave request {RequestId}", requestId);
            
            var request = await _leaveRequestRepository.GetByIdAsync(requestId);
            if (request == null)
            {
                _logger.LogWarning("Cancellation failed: Leave request {RequestId} not found", requestId);
                throw new NotFoundException($"Leave request with ID {requestId} not found");
            }

            if (request.Status != "Pending")
            {
                _logger.LogWarning("Cancellation failed: Request {RequestId} status is {Status}, not Pending", 
                    requestId, request.Status);
                throw new ValidationException("Only pending requests can be cancelled");
            }

            request.Status = "Cancelled";
            await _leaveRequestRepository.UpdateAsync(request);
            _logger.LogInformation("Leave request cancelled: RequestId {RequestId}", requestId);
        }

        public async Task<IEnumerable<LeaveRequest>> GetEmployeeRequestsAsync(int employeeId)
        {
            _logger.LogInformation("Fetching leave requests for employee {EmployeeId}", employeeId);
            var requests = await _leaveRequestRepository.GetRequestsByEmployeeAsync(employeeId);
            _logger.LogInformation("Retrieved {Count} leave requests for employee {EmployeeId}", requests.Count(), employeeId);
            return requests;
        }

        public async Task<IEnumerable<LeaveRequest>> GetPendingRequestsForManagerAsync(int managerId)
        {
            _logger.LogInformation("Fetching pending requests for manager {ManagerId}", managerId);
            var requests = await _leaveRequestRepository.GetPendingRequestsByManagerAsync(managerId);
            _logger.LogInformation("Retrieved {Count} pending requests for manager {ManagerId}", requests.Count(), managerId);
            return requests;
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
