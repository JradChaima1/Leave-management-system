namespace Leave.Core.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public DateTime HireDate { get; set; }
        public int? ManagerId { get; set; }
        public decimal AnnualLeaveBalance { get; set; }
        public decimal SickLeaveBalance { get; set; }
        public bool IsActive { get; set; }

      
        public Employee Manager { get; set; }
        public ICollection<Employee> Subordinates { get; set; }
        public ICollection<LeaveRequest> LeaveRequests { get; set; }
        public ICollection<LeaveBalance> LeaveBalances { get; set; }
    }
}
