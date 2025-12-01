using Microsoft.AspNetCore.Mvc;
using Leave.Core.Interfaces;
using LeaveManagement.Filters;

namespace LeaveManagement.Controllers
{
    [AuthorizeSession]
    public class ProfileController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly ILeaveBalanceService _leaveBalanceService;

        public ProfileController(
            IEmployeeService employeeService,
            ILeaveRequestService leaveRequestService,
            ILeaveBalanceService leaveBalanceService)
        {
            _employeeService = employeeService;
            _leaveRequestService = leaveRequestService;
            _leaveBalanceService = leaveBalanceService;
        }

        public async Task<IActionResult> Index()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            
            try
            {
                var employee = await _employeeService.GetEmployeeWithDetailsAsync(employeeId);
                var leaveBalances = await _leaveBalanceService.GetEmployeeBalancesAsync(employeeId);
                var recentRequests = await _leaveRequestService.GetEmployeeRequestsAsync(employeeId);

                ViewBag.LeaveBalances = leaveBalances;
                ViewBag.RecentRequests = recentRequests.Take(5);

                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
