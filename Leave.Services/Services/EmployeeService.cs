using Leave.Core.Models;
using Leave.Core.Interfaces;

namespace Leave.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new Exception($"Employee with ID {id} not found");
            return employee;
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            
            if (string.IsNullOrEmpty(employee.FirstName))
                throw new Exception("First name is required");
            if (string.IsNullOrEmpty(employee.LastName))
                throw new Exception("Last name is required");
            if (string.IsNullOrEmpty(employee.Email))
                throw new Exception("Email is required");

           
            employee.IsActive = true;
            employee.HireDate = employee.HireDate == default ? DateTime.Now : employee.HireDate;
            employee.AnnualLeaveBalance = 20;
            employee.SickLeaveBalance = 10;

            return await _employeeRepository.AddAsync(employee);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            var existing = await _employeeRepository.GetByIdAsync(employee.EmployeeId);
            if (existing == null)
                throw new Exception($"Employee with ID {employee.EmployeeId} not found");

            await _employeeRepository.UpdateAsync(employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new Exception($"Employee with ID {id} not found");

          
            employee.IsActive = false;
            await _employeeRepository.UpdateAsync(employee);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByManagerAsync(int managerId)
        {
            return await _employeeRepository.GetEmployeesByManagerAsync(managerId);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            return await _employeeRepository.GetEmployeesByDepartmentAsync(department);
        }

        public async Task<Employee> GetEmployeeWithDetailsAsync(int employeeId)
        {
            return await _employeeRepository.GetEmployeeWithLeaveRequestsAsync(employeeId);
        }
    }
}
