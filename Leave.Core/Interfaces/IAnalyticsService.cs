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
    }
}
