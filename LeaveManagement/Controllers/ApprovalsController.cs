using Microsoft.AspNetCore.Mvc;
using Leave.Core.Interfaces;
using LeaveManagement.Filters;

namespace LeaveManagement.Controllers
{
    [AuthorizeSession("Manager")]
    public class ApprovalsController : Controller
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public ApprovalsController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }

        public async Task<IActionResult> Index()
        {
            var managerId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var requests = await _leaveRequestService.GetPendingRequestsForManagerAsync(managerId);
            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var approverId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
                await _leaveRequestService.ApproveRequestAsync(id, approverId);
                TempData["Success"] = "Leave request approved successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            try
            {
                var rejectedBy = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
                await _leaveRequestService.RejectRequestAsync(id, rejectedBy, reason);
                TempData["Success"] = "Leave request rejected successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
