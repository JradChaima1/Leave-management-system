using Microsoft.AspNetCore.Mvc;
using Leave.Core.Interfaces;
using Leave.Core.Models;
using LeaveManagement.Filters;

namespace LeaveManagement.Controllers
{
    [AuthorizeSession("Admin")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View(departments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {
            try
            {
                await _departmentService.CreateDepartmentAsync(department);
                TempData["Success"] = "Department created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(department);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var department = await _departmentService.GetDepartmentByIdAsync(id);
                return View(department);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Department department)
        {
            try
            {
                await _departmentService.UpdateDepartmentAsync(department);
                TempData["Success"] = "Department updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(department);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _departmentService.DeleteDepartmentAsync(id);
                TempData["Success"] = "Department deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
