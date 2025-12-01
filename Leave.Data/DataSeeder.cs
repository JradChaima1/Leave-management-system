using Leave.Core.Models;
using System.Security.Cryptography;
using System.Text;

namespace Leave.Data
{
    public class DataSeeder
    {
        public static void SeedData(LeaveContext context)
        {
            if (context.Roles.Any())
                return;

            var roles = new List<Role>
            {
                new Role { Name = "Admin", Description = "System Administrator" },
                new Role { Name = "Manager", Description = "Department Manager" },
                new Role { Name = "Employee", Description = "Regular Employee" }
            };
            context.Roles.AddRange(roles);
            context.SaveChanges();

            var departments = new List<Department>
            {
                new Department { Name = "IT", Description = "Information Technology" },
                new Department { Name = "HR", Description = "Human Resources" },
                new Department { Name = "Finance", Description = "Finance Department" },
                new Department { Name = "Sales", Description = "Sales Department" },
                new Department { Name = "Marketing", Description = "Marketing Department" }
            };
            context.Departments.AddRange(departments);
            context.SaveChanges();

            var leaveTypes = new List<LeaveType>
            {
                new LeaveType { Name = "Annual Leave", Description = "Paid annual vacation", MaxDaysPerYear = 20, RequiresApproval = true, IsPaid = true },
                new LeaveType { Name = "Sick Leave", Description = "Medical leave", MaxDaysPerYear = 10, RequiresApproval = false, IsPaid = true },
                new LeaveType { Name = "Maternity Leave", Description = "Maternity leave", MaxDaysPerYear = 90, RequiresApproval = true, IsPaid = true },
                new LeaveType { Name = "Unpaid Leave", Description = "Leave without pay", MaxDaysPerYear = 30, RequiresApproval = true, IsPaid = false },
                new LeaveType { Name = "Emergency Leave", Description = "Emergency situations", MaxDaysPerYear = 3, RequiresApproval = false, IsPaid = true }
            };
            context.LeaveTypes.AddRange(leaveTypes);
            context.SaveChanges();

            var employees = new List<Employee>
            {
                new Employee { FirstName = "John", LastName = "Admin", Email = "admin@company.com", PhoneNumber = "1234567890", Department = "IT", Position = "System Admin", HireDate = new DateTime(2020, 1, 1), ManagerId = null, AnnualLeaveBalance = 20, SickLeaveBalance = 10, IsActive = true },
                new Employee { FirstName = "Jane", LastName = "Manager", Email = "manager@company.com", PhoneNumber = "1234567891", Department = "IT", Position = "IT Manager", HireDate = new DateTime(2020, 2, 1), ManagerId = null, AnnualLeaveBalance = 20, SickLeaveBalance = 10, IsActive = true },
                new Employee { FirstName = "Bob", LastName = "Smith", Email = "bob@company.com", PhoneNumber = "1234567892", Department = "IT", Position = "Developer", HireDate = new DateTime(2021, 3, 1), ManagerId = null, AnnualLeaveBalance = 20, SickLeaveBalance = 10, IsActive = true },
                new Employee { FirstName = "Alice", LastName = "Johnson", Email = "alice@company.com", PhoneNumber = "1234567893", Department = "HR", Position = "HR Manager", HireDate = new DateTime(2020, 4, 1), ManagerId = null, AnnualLeaveBalance = 20, SickLeaveBalance = 10, IsActive = true },
                new Employee { FirstName = "Charlie", LastName = "Brown", Email = "charlie@company.com", PhoneNumber = "1234567894", Department = "Finance", Position = "Accountant", HireDate = new DateTime(2021, 5, 1), ManagerId = null, AnnualLeaveBalance = 20, SickLeaveBalance = 10, IsActive = true }
            };
            context.Employees.AddRange(employees);
            context.SaveChanges();

            employees[1].ManagerId = employees[0].EmployeeId;
            employees[2].ManagerId = employees[1].EmployeeId;
            employees[3].ManagerId = employees[0].EmployeeId;
            employees[4].ManagerId = employees[0].EmployeeId;
            context.SaveChanges();

            string passwordHash = HashPassword("password");

            var users = new List<User>
            {
                new User { Username = "admin", PasswordHash = passwordHash, Email = "admin@company.com", RoleId = roles[0].RoleId, EmployeeId = employees[0].EmployeeId, IsActive = true, CreatedDate = DateTime.Now },
                new User { Username = "manager", PasswordHash = passwordHash, Email = "manager@company.com", RoleId = roles[1].RoleId, EmployeeId = employees[1].EmployeeId, IsActive = true, CreatedDate = DateTime.Now },
                new User { Username = "employee", PasswordHash = passwordHash, Email = "bob@company.com", RoleId = roles[2].RoleId, EmployeeId = employees[2].EmployeeId, IsActive = true, CreatedDate = DateTime.Now }
            };
            context.Users.AddRange(users);
            context.SaveChanges();

            var holidays = new List<Holiday>
            {
                new Holiday { Name = "New Year", Date = new DateTime(2025, 1, 1), IsRecurring = true, Description = "New Year's Day" },
                new Holiday { Name = "Christmas", Date = new DateTime(2025, 12, 25), IsRecurring = true, Description = "Christmas Day" },
                new Holiday { Name = "Independence Day", Date = new DateTime(2025, 7, 4), IsRecurring = true, Description = "Independence Day" }
            };
            context.Holidays.AddRange(holidays);
            context.SaveChanges();

            var leaveBalances = new List<LeaveBalance>();
            foreach (var employee in employees)
            {
                foreach (var leaveType in leaveTypes)
                {
                    leaveBalances.Add(new LeaveBalance
                    {
                        EmployeeId = employee.EmployeeId,
                        LeaveTypeId = leaveType.LeaveTypeId,
                        Year = 2025,
                        TotalDays = leaveType.MaxDaysPerYear,
                        UsedDays = 0,
                        RemainingDays = leaveType.MaxDaysPerYear
                    });
                }
            }
            context.LeaveBalances.AddRange(leaveBalances);
            context.SaveChanges();

            var leaveRequests = new List<LeaveRequest>
            {
                new LeaveRequest { EmployeeId = 3, LeaveTypeId = 1, StartDate = new DateTime(2025, 11, 10), EndDate = new DateTime(2025, 11, 15), TotalDays = 5, Reason = "Family vacation", Status = "Approved", RequestDate = new DateTime(2025, 10, 20), ApprovedBy = 2, ApprovalDate = new DateTime(2025, 10, 21) },
                new LeaveRequest { EmployeeId = 3, LeaveTypeId = 2, StartDate = new DateTime(2025, 10, 5), EndDate = new DateTime(2025, 10, 6), TotalDays = 2, Reason = "Medical checkup", Status = "Approved", RequestDate = new DateTime(2025, 10, 1), ApprovedBy = 2, ApprovalDate = new DateTime(2025, 10, 2) },
                new LeaveRequest { EmployeeId = 5, LeaveTypeId = 1, StartDate = new DateTime(2025, 12, 20), EndDate = new DateTime(2025, 12, 30), TotalDays = 8, Reason = "Year-end holiday", Status = "Pending", RequestDate = DateTime.Now },
                new LeaveRequest { EmployeeId = 4, LeaveTypeId = 1, StartDate = new DateTime(2025, 11, 1), EndDate = new DateTime(2025, 11, 5), TotalDays = 4, Reason = "Personal leave", Status = "Approved", RequestDate = new DateTime(2025, 10, 15), ApprovedBy = 1, ApprovalDate = new DateTime(2025, 10, 16) },
                new LeaveRequest { EmployeeId = 5, LeaveTypeId = 2, StartDate = new DateTime(2025, 10, 10), EndDate = new DateTime(2025, 10, 11), TotalDays = 2, Reason = "Flu", Status = "Rejected", RequestDate = new DateTime(2025, 10, 8), ApprovedBy = 1, ApprovalDate = new DateTime(2025, 10, 9), RejectionReason = "Insufficient documentation" },
                new LeaveRequest { EmployeeId = 3, LeaveTypeId = 5, StartDate = new DateTime(2025, 9, 15), EndDate = new DateTime(2025, 9, 15), TotalDays = 1, Reason = "Family emergency", Status = "Approved", RequestDate = new DateTime(2025, 9, 15), ApprovedBy = 2, ApprovalDate = new DateTime(2025, 9, 15) },
                new LeaveRequest { EmployeeId = 4, LeaveTypeId = 2, StartDate = new DateTime(2025, 12, 5), EndDate = new DateTime(2025, 12, 7), TotalDays = 3, Reason = "Doctor appointment", Status = "Pending", RequestDate = DateTime.Now },
                new LeaveRequest { EmployeeId = 5, LeaveTypeId = 1, StartDate = new DateTime(2025, 8, 10), EndDate = new DateTime(2025, 8, 20), TotalDays = 8, Reason = "Summer vacation", Status = "Approved", RequestDate = new DateTime(2025, 7, 15), ApprovedBy = 1, ApprovalDate = new DateTime(2025, 7, 16) },
                new LeaveRequest { EmployeeId = 3, LeaveTypeId = 1, StartDate = new DateTime(2025, 7, 1), EndDate = new DateTime(2025, 7, 5), TotalDays = 4, Reason = "Wedding anniversary", Status = "Approved", RequestDate = new DateTime(2025, 6, 10), ApprovedBy = 2, ApprovalDate = new DateTime(2025, 6, 11) },
                new LeaveRequest { EmployeeId = 4, LeaveTypeId = 4, StartDate = new DateTime(2025, 12, 15), EndDate = new DateTime(2025, 12, 20), TotalDays = 5, Reason = "Personal matters", Status = "Pending", RequestDate = DateTime.Now }
            };
            context.LeaveRequests.AddRange(leaveRequests);
            context.SaveChanges();

            var balancesToUpdate = context.LeaveBalances.Where(b => 
                (b.EmployeeId == 3 && (b.LeaveTypeId == 1 || b.LeaveTypeId == 2 || b.LeaveTypeId == 5)) ||
                (b.EmployeeId == 4 && b.LeaveTypeId == 1) ||
                (b.EmployeeId == 5 && b.LeaveTypeId == 1)).ToList();

            foreach (var balance in balancesToUpdate)
            {
                if (balance.EmployeeId == 3 && balance.LeaveTypeId == 1)
                {
                    balance.UsedDays = 9;
                    balance.RemainingDays = 11;
                }
                else if (balance.EmployeeId == 3 && balance.LeaveTypeId == 2)
                {
                    balance.UsedDays = 2;
                    balance.RemainingDays = 8;
                }
                else if (balance.EmployeeId == 3 && balance.LeaveTypeId == 5)
                {
                    balance.UsedDays = 1;
                    balance.RemainingDays = 2;
                }
                else if (balance.EmployeeId == 4 && balance.LeaveTypeId == 1)
                {
                    balance.UsedDays = 4;
                    balance.RemainingDays = 16;
                }
                else if (balance.EmployeeId == 5 && balance.LeaveTypeId == 1)
                {
                    balance.UsedDays = 8;
                    balance.RemainingDays = 12;
                }
            }
            context.SaveChanges();
        }
private static string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password);
}

      
    }
}
