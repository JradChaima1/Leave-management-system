using Leave.Core.Models;

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
                new Role { RoleId = 1, Name = "Admin", Description = "System Administrator" },
                new Role { RoleId = 2, Name = "Manager", Description = "Department Manager" },
                new Role { RoleId = 3, Name = "Employee", Description = "Regular Employee" }
            };
            context.Roles.AddRange(roles);
            context.SaveChanges();

            var departments = new List<Department>
            {
                new Department { DepartmentId = 1, Name = "IT", Description = "Information Technology" },
                new Department { DepartmentId = 2, Name = "HR", Description = "Human Resources" },
                new Department { DepartmentId = 3, Name = "Finance", Description = "Finance Department" },
                new Department { DepartmentId = 4, Name = "Sales", Description = "Sales Department" },
                new Department { DepartmentId = 5, Name = "Marketing", Description = "Marketing Department" }
            };
            context.Departments.AddRange(departments);
            context.SaveChanges();

            var leaveTypes = new List<LeaveType>
            {
                new LeaveType { LeaveTypeId = 1, Name = "Annual Leave", Description = "Paid annual vacation", MaxDaysPerYear = 20, RequiresApproval = true, IsPaid = true },
                new LeaveType { LeaveTypeId = 2, Name = "Sick Leave", Description = "Medical leave", MaxDaysPerYear = 10, RequiresApproval = false, IsPaid = true },
                new LeaveType { LeaveTypeId = 3, Name = "Maternity Leave", Description = "Maternity leave", MaxDaysPerYear = 90, RequiresApproval = true, IsPaid = true },
                new LeaveType { LeaveTypeId = 4, Name = "Unpaid Leave", Description = "Leave without pay", MaxDaysPerYear = 30, RequiresApproval = true, IsPaid = false },
                new LeaveType { LeaveTypeId = 5, Name = "Emergency Leave", Description = "Emergency situations", MaxDaysPerYear = 3, RequiresApproval = false, IsPaid = true }
            };
            context.LeaveTypes.AddRange(leaveTypes);
            context.SaveChanges();

            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, FirstName = "John", LastName = "Admin", Email = "admin@company.com", PhoneNumber = "1234567890", Department = "IT", Position = "System Admin", HireDate = new DateTime(2020, 1, 1), ManagerId = null, AnnualLeaveBalance = 20, SickLeaveBalance = 10, IsActive = true },
                new Employee { EmployeeId = 2, FirstName = "Jane", LastName = "Manager", Email = "manager@company.com", PhoneNumber = "1234567891", Department = "IT", Position = "IT Manager", HireDate = new DateTime(2020, 2, 1), ManagerId = 1, AnnualLeaveBalance = 20, SickLeaveBalance = 10, IsActive = true },
                new Employee { EmployeeId = 3, FirstName = "Bob", LastName = "Smith", Email = "bob@company.com", PhoneNumber = "1234567892", Department = "IT", Position = "Developer", HireDate = new DateTime(2021, 3, 1), ManagerId = 2, AnnualLeaveBalance = 20, SickLeaveBalance = 10, IsActive = true },
                new Employee { EmployeeId = 4, FirstName = "Alice", LastName = "Johnson", Email = "alice@company.com", PhoneNumber = "1234567893", Department = "HR", Position = "HR Manager", HireDate = new DateTime(2020, 4, 1), ManagerId = 1, AnnualLeaveBalance = 20, SickLeaveBalance = 10, IsActive = true },
                new Employee { EmployeeId = 5, FirstName = "Charlie", LastName = "Brown", Email = "charlie@company.com", PhoneNumber = "1234567894", Department = "Finance", Position = "Accountant", HireDate = new DateTime(2021, 5, 1), ManagerId = 1, AnnualLeaveBalance = 20, SickLeaveBalance = 10, IsActive = true }
            };
            context.Employees.AddRange(employees);
            context.SaveChanges();

            var users = new List<User>
            {
                new User { UserId = 1, Username = "admin", PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", Email = "admin@company.com", RoleId = 1, EmployeeId = 1, IsActive = true, CreatedDate = DateTime.Now },
                new User { UserId = 2, Username = "manager", PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", Email = "manager@company.com", RoleId = 2, EmployeeId = 2, IsActive = true, CreatedDate = DateTime.Now },
                new User { UserId = 3, Username = "employee", PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", Email = "bob@company.com", RoleId = 3, EmployeeId = 3, IsActive = true, CreatedDate = DateTime.Now }
            };
            context.Users.AddRange(users);
            context.SaveChanges();

            var holidays = new List<Holiday>
            {
                new Holiday { HolidayId = 1, Name = "New Year", Date = new DateTime(2025, 1, 1), IsRecurring = true, Description = "New Year's Day" },
                new Holiday { HolidayId = 2, Name = "Christmas", Date = new DateTime(2025, 12, 25), IsRecurring = true, Description = "Christmas Day" },
                new Holiday { HolidayId = 3, Name = "Independence Day", Date = new DateTime(2025, 7, 4), IsRecurring = true, Description = "Independence Day" }
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
        }
    }
}
