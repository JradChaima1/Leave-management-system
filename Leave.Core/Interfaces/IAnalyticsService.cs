namespace Leave.Core.Interfaces
{
    public interface IAnalyticsService
    {
        Task<int> GetTotalEmployeesAsync();
        Task<int> GetPendingRequestsCountAsync();
        Task<int> GetApprovedRequestsThisMonthAsync();
        Task<int> GetRejectedRequestsThisMonthAsync();
        Task<decimal> GetAverageLeaveDaysAsync();
        Task<decimal> GetCurrentAvailabilityRateAsync();
        Task<Dictionary<string, int>> GetLeaveRequestsByStatusAsync();
        Task<Dictionary<string, int>> GetLeaveUsageByDepartmentAsync();
        Task<Dictionary<string, int>> GetLeaveTypeDistributionAsync();
        Task<int> GetTotalEmployeesByDepartmentAsync(int managerId);
        Task<int> GetPendingRequestsCountByDepartmentAsync(int managerId);
        Task<int> GetApprovedRequestsThisMonthByDepartmentAsync(int managerId);
        Task<int> GetRejectedRequestsThisMonthByDepartmentAsync(int managerId);
        Task<decimal> GetAverageLeaveDaysByDepartmentAsync(int managerId);
        Task<decimal> GetCurrentAvailabilityRateByDepartmentAsync(int managerId);
        Task<Dictionary<string, int>> GetLeaveRequestsByStatusByDepartmentAsync(int managerId);
        Task<Dictionary<string, int>> GetLeaveTypeDistributionByDepartmentAsync(int managerId);
    }
}
