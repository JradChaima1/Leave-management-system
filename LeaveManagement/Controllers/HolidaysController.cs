using Microsoft.AspNetCore.Mvc;
using Leave.Core.Interfaces;
using Leave.Core.Models;
using LeaveManagement.Filters;

namespace LeaveManagement.Controllers
{
    [AuthorizeSession("Admin")]
    public class HolidaysController : Controller
    {
        private readonly IHolidayService _holidayService;

        public HolidaysController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        public async Task<IActionResult> Index()
        {
            var holidays = await _holidayService.GetAllHolidaysAsync();
            return View(holidays);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Holiday holiday)
        {
            try
            {
                await _holidayService.CreateHolidayAsync(holiday);
                TempData["Success"] = "Holiday created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(holiday);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var holiday = await _holidayService.GetHolidayByIdAsync(id);
                return View(holiday);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Holiday holiday)
        {
            try
            {
                await _holidayService.UpdateHolidayAsync(holiday);
                TempData["Success"] = "Holiday updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(holiday);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _holidayService.DeleteHolidayAsync(id);
                TempData["Success"] = "Holiday deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
