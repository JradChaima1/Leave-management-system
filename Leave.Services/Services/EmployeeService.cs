using Leave.Core.Models;
using Leave.Core.Interfaces;
using Leave.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace Leave.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRepository<LeaveType> _leaveTypeRepository;
        private readonly IRepository<LeaveBalance> _leaveBalanceRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository employeeRepository, 
            IRepository<LeaveType> leaveTypeRepository,
            IRepository<LeaveBalance> leaveBalanceRepository,
            ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _leaveBalanceRepository = leaveBalanceRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            _logger.LogInformation("Fetching all employees");
            var employees = await _employeeRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} employees", employees.Count());
            return employees;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            _logger.LogInformation("Fetching employee with ID: {EmployeeId}", id);
            var employee = await _employeeRepository.GetByIdAsync(id);
            
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found", id);
                throw new NotFoundException($"Employee with ID {id} not found");
            }
            
            return employee;
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            _logger.LogInformation("Creating employee: {Email}", employee.Email);
            
            if (string.IsNullOrEmpty(employee.FirstName))
            {
                _logger.LogWarning("Employee creation failed: First name is required");
                throw new ValidationException("First name is required");
            }
            if (string.IsNullOrEmpty(employee.LastName))
            {
                _logger.LogWarning("Employee creation failed: Last name is required");
                throw new ValidationException("Last name is required");
            }
            if (string.IsNullOrEmpty(employee.Email))
            {
                _logger.LogWarning("Employee creation failed: Email is required");
                throw new ValidationException("Email is required");
            }

            employee.IsActive = true;
            employee.HireDate = employee.HireDate == default ? DateTime.Now : employee.HireDate;
            employee.AnnualLeaveBalance = 20;
            employee.SickLeaveBalance = 10;

            var result = await _employeeRepository.AddAsync(employee);
            _logger.LogInformation("Employee created successfully: {EmployeeId}, {Email}", result.EmployeeId, result.Email);
            
           
            var leaveTypes = await _leaveTypeRepository.GetAllAsync();
            var currentYear = DateTime.Now.Year;
            
            foreach (var leaveType in leaveTypes)
            {
                var leaveBalance = new LeaveBalance
                {
                    EmployeeId = result.EmployeeId,
                    LeaveTypeId = leaveType.LeaveTypeId,
                    Year = currentYear,
                    TotalDays = leaveType.MaxDaysPerYear,
                    UsedDays = 0,
                    RemainingDays = leaveType.MaxDaysPerYear
                };
                await _leaveBalanceRepository.AddAsync(leaveBalance);
            }
            
            _logger.LogInformation("Leave balances created for employee {EmployeeId}", result.EmployeeId);
            
            return result;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _logger.LogInformation("Updating employee: {EmployeeId}", employee.EmployeeId);
            
            var existing = await _employeeRepository.GetByIdAsync(employee.EmployeeId);
            if (existing == null)
            {
                _logger.LogWarning("Update failed: Employee with ID {EmployeeId} not found", employee.EmployeeId);
                throw new NotFoundException($"Employee with ID {employee.EmployeeId} not found");
            }

            existing.FirstName = employee.FirstName;
            existing.LastName = employee.LastName;
            existing.Email = employee.Email;
            existing.PhoneNumber = employee.PhoneNumber;
            existing.Department = employee.Department;
            existing.Position = employee.Position;
            existing.HireDate = employee.HireDate;
            existing.IsActive = employee.IsActive;

            await _employeeRepository.UpdateAsync(existing);
            _logger.LogInformation("Employee updated successfully: {EmployeeId}", employee.EmployeeId);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            _logger.LogInformation("Deleting employee: {EmployeeId}", id);
            
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Delete failed: Employee with ID {EmployeeId} not found", id);
                throw new NotFoundException($"Employee with ID {id} not found");
            }

            employee.IsActive = false;
            await _employeeRepository.UpdateAsync(employee);
            _logger.LogInformation("Employee soft-deleted successfully: {EmployeeId}", id);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByManagerAsync(int managerId)
        {
            _logger.LogInformation("Fetching employees for manager: {ManagerId}", managerId);
            var employees = await _employeeRepository.GetEmployeesByManagerAsync(managerId);
            _logger.LogInformation("Retrieved {Count} employees for manager {ManagerId}", employees.Count(), managerId);
            return employees;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            _logger.LogInformation("Fetching employees for department: {Department}", department);
            var employees = await _employeeRepository.GetEmployeesByDepartmentAsync(department);
            _logger.LogInformation("Retrieved {Count} employees for department {Department}", employees.Count(), department);
            return employees;
        }

        public async Task<Employee> GetEmployeeWithDetailsAsync(int employeeId)
        {
            _logger.LogInformation("Fetching employee details: {EmployeeId}", employeeId);
            var employee = await _employeeRepository.GetEmployeeWithLeaveRequestsAsync(employeeId);
            
            if (employee == null)
            {
                _logger.LogWarning("Employee details not found: {EmployeeId}", employeeId);
                throw new NotFoundException($"Employee with ID {employeeId} not found");
            }
            
            return employee;
        }
    }
}
