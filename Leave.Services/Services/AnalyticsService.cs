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
            var requests = await _leaveRequestRepository.GetRequestsByStatusAsync("Approved");
            
            return requests.Where(r => r.Employee != null)
                          .GroupBy(r => r.Employee.Department)
                          .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetLeaveTypeDistributionAsync()
        {
            var requests = await _leaveRequestRepository.GetRequestsByStatusAsync("Approved");
            var pendingRequests = await _leaveRequestRepository.GetRequestsByStatusAsync("Pending");
            var rejectedRequests = await _leaveRequestRepository.GetRequestsByStatusAsync("Rejected");
            
            var allRequests = requests.Concat(pendingRequests).Concat(rejectedRequests);
            
            return allRequests.Where(r => r.LeaveType != null)
                          .GroupBy(r => r.LeaveType.Name)
                          .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<int> GetTotalEmployeesByDepartmentAsync(int managerId)
        {
            var manager = await _employeeRepository.GetByIdAsync(managerId);
            if (manager == null) return 0;
            
            var employees = await _employeeRepository.GetEmployeesByDepartmentAsync(manager.Department);
            return employees.Count(e => e.IsActive);
        }

        public async Task<int> GetPendingRequestsCountByDepartmentAsync(int managerId)
        {
            var manager = await _employeeRepository.GetByIdAsync(managerId);
            if (manager == null) return 0;
            
            var requests = await _leaveRequestRepository.GetRequestsByStatusAsync("Pending");
            return requests.Count(r => r.Employee != null && r.Employee.Department == manager.Department);
        }

        public async Task<int> GetApprovedRequestsThisMonthByDepartmentAsync(int managerId)
        {
            var manager = await _employeeRepository.GetByIdAsync(managerId);
            if (manager == null) return 0;
            
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var requests = await _leaveRequestRepository.GetRequestsByDateRangeAsync(startOfMonth, endOfMonth);
            return requests.Count(r => r.Status == "Approved" && r.Employee != null && r.Employee.Department == manager.Department);
        }

        public async Task<int> GetRejectedRequestsThisMonthByDepartmentAsync(int managerId)
        {
            var manager = await _employeeRepository.GetByIdAsync(managerId);
            if (manager == null) return 0;
            
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var requests = await _leaveRequestRepository.GetRequestsByDateRangeAsync(startOfMonth, endOfMonth);
            return requests.Count(r => r.Status == "Rejected" && r.Employee != null && r.Employee.Department == manager.Department);
        }

        public async Task<decimal> GetAverageLeaveDaysByDepartmentAsync(int managerId)
        {
            var manager = await _employeeRepository.GetByIdAsync(managerId);
            if (manager == null) return 0;
            
            var requests = await _leaveRequestRepository.GetAllAsync();
            var departmentRequests = requests.Where(r => r.Status == "Approved" && r.Employee != null && r.Employee.Department == manager.Department);
            return departmentRequests.Any() ? departmentRequests.Average(r => r.TotalDays) : 0;
        }

        public async Task<decimal> GetCurrentAvailabilityRateByDepartmentAsync(int managerId)
        {
            var totalEmployees = await GetTotalEmployeesByDepartmentAsync(managerId);
            if (totalEmployees == 0) return 100;

            var manager = await _employeeRepository.GetByIdAsync(managerId);
            if (manager == null) return 100;

            var today = DateTime.Now.Date;
            var requests = await _leaveRequestRepository.GetAllAsync();
            var onLeaveToday = requests.Count(r => 
                r.Status == "Approved" && 
                r.StartDate.Date <= today && 
                r.EndDate.Date >= today &&
                r.Employee != null &&
                r.Employee.Department == manager.Department);

            return ((totalEmployees - onLeaveToday) / (decimal)totalEmployees) * 100;
        }

        public async Task<Dictionary<string, int>> GetLeaveRequestsByStatusByDepartmentAsync(int managerId)
        {
            var manager = await _employeeRepository.GetByIdAsync(managerId);
            if (manager == null) return new Dictionary<string, int>();
            
            var requests = await _leaveRequestRepository.GetAllAsync();
            var departmentRequests = requests.Where(r => r.Employee != null && r.Employee.Department == manager.Department);
            return departmentRequests.GroupBy(r => r.Status)
                          .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<string, int>> GetLeaveTypeDistributionByDepartmentAsync(int managerId)
        {
            var manager = await _employeeRepository.GetByIdAsync(managerId);
            if (manager == null) return new Dictionary<string, int>();
            
            var approved = await _leaveRequestRepository.GetRequestsByStatusAsync("Approved");
            var pending = await _leaveRequestRepository.GetRequestsByStatusAsync("Pending");
            var rejected = await _leaveRequestRepository.GetRequestsByStatusAsync("Rejected");
            
            var allRequests = approved.Concat(pending).Concat(rejected);
            var departmentRequests = allRequests.Where(r => r.Employee != null && r.Employee.Department == manager.Department && r.LeaveType != null);
            
            return departmentRequests.GroupBy(r => r.LeaveType.Name)
                          .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
