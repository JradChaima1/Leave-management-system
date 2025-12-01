using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Leave.Core.Interfaces;
using Leave.Core.Models;
using LeaveManagement.Filters;

namespace LeaveManagement.Controllers
{
    [AuthorizeSession("Admin", "Manager")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IAuthService _authService;

        public EmployeesController(IEmployeeService employeeService, IDepartmentService departmentService, IAuthService authService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var employeeId = HttpContext.Session.GetInt32("EmployeeId");
            
            IEnumerable<Employee> employees;
            
            if (roleId == 2 && employeeId.HasValue)
            {
                var manager = await _employeeService.GetEmployeeByIdAsync(employeeId.Value);
                employees = await _employeeService.GetEmployeesByDepartmentAsync(manager.Department);
            }
            else
            {
                employees = await _employeeService.GetAllEmployeesAsync();
            }
            
            var users = await _authService.GetAllUsersAsync();
            ViewBag.Users = users;
            
            return View(employees);
        }

        [AuthorizeSession("Admin", "Manager")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeWithDetailsAsync(id);
                
                // If Manager, verify the employee is in their department
                var roleId = HttpContext.Session.GetInt32("RoleId");
                var employeeId = HttpContext.Session.GetInt32("EmployeeId");
                
                if (roleId == 2 && employeeId.HasValue)
                {
                    var manager = await _employeeService.GetEmployeeByIdAsync(employeeId.Value);
                    if (employee.Department != manager.Department)
                    {
                        TempData["Error"] = "You can only view employees from your department";
                        return RedirectToAction("Index");
                    }
                }
                
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
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Departments = new SelectList(departments, "Name", "Name");
            return View();
        }

        [HttpPost]
        [AuthorizeSession("Admin")]
        public async Task<IActionResult> Create(Employee employee, int RoleId, string username, string password)
        {
            try
            {
                var createdEmployee = await _employeeService.CreateEmployeeAsync(employee);
                
                var user = new User
                {
                    Username = username ?? employee.Email.Split('@')[0],
                    Email = employee.Email,
                    RoleId = RoleId,
                    EmployeeId = createdEmployee.EmployeeId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };
                
                await _authService.RegisterAsync(user, password ?? "password");
                
                TempData["Success"] = "Employee and user account created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                var departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.Departments = new SelectList(departments, "Name", "Name");
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
                var departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.Departments = new SelectList(departments, "Name", "Name", employee.Department);
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
                var departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.Departments = new SelectList(departments, "Name", "Name", employee.Department);
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
