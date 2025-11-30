using Leave.Core.Interfaces;

namespace Leave.Services.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public AnalyticsService(
            IEmployeeRepository employeeRepository,
            ILeaveRequestRepository leaveRequestRepository)
        {
            _employeeRepository = employeeRepository;
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<int> GetTotalEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Count(e => e.IsActive);
        }

        public async Task<int> GetPendingRequestsCountAsync()
        {
            var requests = await _leaveRequestRepository.GetRequestsByStatusAsync("Pending");
            return requests.Count();
        }

        public async Task<int> GetApprovedRequestsThisMonthAsync()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var requests = await _leaveRequestRepository.GetRequestsByDateRangeAsync(startOfMonth, endOfMonth);
            return requests.Count(r => r.Status == "Approved");
        }

        public async Task<int> GetRejectedRequestsThisMonthAsync()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var requests = await _leaveRequestRepository.GetRequestsByDateRangeAsync(startOfMonth, endOfMonth);
            return requests.Count(r => r.Status == "Rejected");
        }

        public async Task<decimal> GetAverageLeaveDaysAsync()
        {
            var requests = await _leaveRequestRepository.GetAllAsync();
            var approvedRequests = requests.Where(r => r.Status == "Approved");
            return approvedRequests.Any() ? approvedRequests.Average(r => r.TotalDays) : 0;
        }

        public async Task<decimal> GetCurrentAvailabilityRateAsync()
        {
            var totalEmployees = await GetTotalEmployeesAsync();
            if (totalEmployees == 0) return 100;

            var today = DateTime.Now.Date;
            var requests = await _leaveRequestRepository.GetAllAsync();
            var onLeaveToday = requests.Count(r => 
                r.Status == "Approved" && 
                r.StartDate.Date <= today && 
                r.EndDate.Date >= today);

            return ((totalEmployees - onLeaveToday) / (decimal)totalEmployees) * 100;
        }

        public async Task<Dictionary<string, int>> GetLeaveRequestsByStatusAsync()
        {
            var requests = await _leaveRequestRepository.GetAllAsync();
            return requests.GroupBy(r => r.Status)
                          .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetLeaveUsageByDepartmentAsync()
        {
            var requests = await _leaveRequestRepository.GetAllAsync();
            var approvedRequests = requests.Where(r => r.Status == "Approved");
            
            return approvedRequests.GroupBy(r => r.Employee.Department)
                                  .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetLeaveTypeDistributionAsync()
        {
            var requests = await _leaveRequestRepository.GetAllAsync();
            return requests.GroupBy(r => r.LeaveType.Name)
                          .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
