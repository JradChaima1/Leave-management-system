using Microsoft.AspNetCore.Mvc;
using Leave.Core.Interfaces;
using Leave.Core.Models;
using LeaveManagement.Filters;

namespace LeaveManagement.Controllers
{
    [AuthorizeSession("Admin", "Manager")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeWithDetailsAsync(id);
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [AuthorizeSession("Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AuthorizeSession("Admin")]
        public async Task<IActionResult> Create(Employee employee)
        {
            try
            {
                await _employeeService.CreateEmployeeAsync(employee);
                TempData["Success"] = "Employee created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(employee);
            }
        }

        [HttpGet]
        [AuthorizeSession("Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [AuthorizeSession("Admin")]
        public async Task<IActionResult> Edit(Employee employee)
        {
            try
            {
                await _employeeService.UpdateEmployeeAsync(employee);
                TempData["Success"] = "Employee updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(employee);
            }
        }

        [AuthorizeSession("Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                TempData["Success"] = "Employee deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
