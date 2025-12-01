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
            var roleId = HttpContext.Session.GetInt32("RoleId") ?? 0;
            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;

            if (roleId == 1)
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
            }
            else if (roleId == 2)
            {
                ViewBag.TotalEmployees = await _analyticsService.GetTotalEmployeesByDepartmentAsync(employeeId);
                ViewBag.PendingRequests = await _analyticsService.GetPendingRequestsCountByDepartmentAsync(employeeId);
                ViewBag.ApprovedThisMonth = await _analyticsService.GetApprovedRequestsThisMonthByDepartmentAsync(employeeId);
                ViewBag.RejectedThisMonth = await _analyticsService.GetRejectedRequestsThisMonthByDepartmentAsync(employeeId);
                ViewBag.AverageLeaveDays = await _analyticsService.GetAverageLeaveDaysByDepartmentAsync(employeeId);
                ViewBag.AvailabilityRate = await _analyticsService.GetCurrentAvailabilityRateByDepartmentAsync(employeeId);

                ViewBag.RequestsByStatus = await _analyticsService.GetLeaveRequestsByStatusByDepartmentAsync(employeeId);
                ViewBag.UsageByDepartment = new Dictionary<string, int>();
                ViewBag.LeaveTypeDistribution = await _analyticsService.GetLeaveTypeDistributionByDepartmentAsync(employeeId);
            }

            return View();
        }
    }
}
