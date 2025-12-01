using Microsoft.AspNetCore.Mvc;
using Leave.Core.Interfaces;
using LeaveManagement.Filters;

namespace LeaveManagement.Controllers
{
    [AuthorizeSession("Admin", "Manager")]
    public class DashboardController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public DashboardController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalEmployees = await _analyticsService.GetTotalEmployeesAsync();
            ViewBag.PendingRequests = await _analyticsService.GetPendingRequestsCountAsync();
            ViewBag.ApprovedThisMonth = await _analyticsService.GetApprovedRequestsThisMonthAsync();
            ViewBag.RejectedThisMonth = await _analyticsService.GetRejectedRequestsThisMonthAsync();
            ViewBag.AverageLeaveDays = await _analyticsService.GetAverageLeaveDaysAsync();
            ViewBag.AvailabilityRate = await _analyticsService.GetCurrentAvailabilityRateAsync();

            ViewBag.RequestsByStatus = await _analyticsService.GetLeaveRequestsByStatusAsync();
            ViewBag.UsageByDepartment = await _analyticsService.GetLeaveUsageByDepartmentAsync();
            ViewBag.LeaveTypeDistribution = await _analyticsService.GetLeaveTypeDistributionAsync();

            return View();
        }
    }
}
