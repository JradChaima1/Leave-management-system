using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Leave.Core.Interfaces;
using Leave.Core.Models;
using LeaveManagement.Filters;

namespace LeaveManagement.Controllers
{
    [AuthorizeSession]
    public class LeaveRequestsController : Controller
    {
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly IRepository<LeaveType> _leaveTypeRepository;

        public LeaveRequestsController(
            ILeaveRequestService leaveRequestService,
            IRepository<LeaveType> leaveTypeRepository)
        {
            _leaveRequestService = leaveRequestService;
            _leaveTypeRepository = leaveTypeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var requests = await _leaveRequestService.GetEmployeeRequestsAsync(employeeId);
            return View(requests);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var leaveTypes = await _leaveTypeRepository.GetAllAsync();
            ViewBag.LeaveTypes = new SelectList(leaveTypes, "LeaveTypeId", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LeaveRequest request)
        {
            try
            {
                request.EmployeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
                await _leaveRequestService.SubmitLeaveRequestAsync(request);
                TempData["Success"] = "Leave request submitted successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var leaveTypes = await _leaveTypeRepository.GetAllAsync();
                ViewBag.LeaveTypes = new SelectList(leaveTypes, "LeaveTypeId", "Name");
                ViewBag.Error = ex.Message;
                return View(request);
            }
        }

        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _leaveRequestService.CancelRequestAsync(id);
                TempData["Success"] = "Leave request cancelled successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
