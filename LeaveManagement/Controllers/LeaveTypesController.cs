using Microsoft.AspNetCore.Mvc;
using Leave.Core.Interfaces;
using Leave.Core.Models;
using LeaveManagement.Filters;

namespace LeaveManagement.Controllers
{
    [AuthorizeSession("Admin")]
    public class LeaveTypesController : Controller
    {
        private readonly IRepository<LeaveType> _leaveTypeRepository;

        public LeaveTypesController(IRepository<LeaveType> leaveTypeRepository)
        {
            _leaveTypeRepository = leaveTypeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var leaveTypes = await _leaveTypeRepository.GetAllAsync();
            return View(leaveTypes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LeaveType leaveType)
        {
            try
            {
                await _leaveTypeRepository.AddAsync(leaveType);
                TempData["Success"] = "Leave type created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(leaveType);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var leaveType = await _leaveTypeRepository.GetByIdAsync(id);
                if (leaveType == null)
                {
                    TempData["Error"] = "Leave type not found";
                    return RedirectToAction("Index");
                }
                return View(leaveType);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LeaveType leaveType)
        {
            try
            {
                await _leaveTypeRepository.UpdateAsync(leaveType);
                TempData["Success"] = "Leave type updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(leaveType);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _leaveTypeRepository.DeleteAsync(id);
                TempData["Success"] = "Leave type deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
