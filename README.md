# Leave Management System

A comprehensive employee leave management system built with ASP.NET Core MVC, featuring role-based access control, leave request workflows, and analytics dashboards.

## Features

### Authentication & Authorization
- Secure login system with BCrypt password hashing
- Role-based access control (Admin, Manager, Employee)
- Session-based authentication
- Custom authorization filters

### Employee Management
- Complete CRUD operations for employees
- Department-based organization
- Manager-subordinate relationships
- Automatic leave balance initialization
- User account creation with employee records

### Leave Request Management
- Submit leave requests with date ranges
- Automatic calculation of working days (excludes weekends and holidays)
- Leave balance validation
- Request status tracking (Pending, Approved, Rejected, Cancelled)
- Attachment support
- Multiple leave types (Annual, Sick, Maternity, Unpaid, Emergency)

### Approval Workflow
- Managers approve requests from their department employees
- Approval/rejection with reason tracking
- Real-time balance updates upon approval
- Request history and audit trail

### Analytics Dashboard
- Role-specific dashboards
- Admin: System-wide statistics and charts
- Manager: Department-specific metrics
- Employee: Personal leave summary
- Interactive charts using Chart.js
- Key metrics: total employees, pending requests, approval rates, availability

### Additional Features
- Holiday management
- Department management
- Leave type configuration
- Leave balance tracking per employee per year
- Professional UI with Bootstrap 5
- Responsive design
- Custom exception handling middleware
- Comprehensive logging

## Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 10.0)
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: BCrypt.Net for password hashing
- **UI**: Bootstrap 5, Bootstrap Icons, Chart.js
- **Architecture**: 4-layer architecture (Core, Data, Services, Presentation)
- **Patterns**: Repository Pattern, Dependency Injection, MVC

## Project Structure

```
LeaveManagement/
├── Leave.Core/              # Domain models and interfaces
│   ├── Models/              # Entity models
│   ├── Interfaces/          # Service and repository interfaces
│   └── Exceptions/          # Custom exceptions
├── Leave.Data/              # Data access layer
│   ├── Repositories/        # Repository implementations
│   ├── Migrations/          # EF Core migrations
│   ├── LeaveContext.cs      # DbContext
│   └── DataSeeder.cs        # Initial data seeding
├── Leave.Services/          # Business logic layer
│   └── Services/            # Service implementations
└── LeaveManagement/         # Presentation layer
    ├── Controllers/         # MVC controllers
    ├── Views/               # Razor views
    ├── Filters/             # Custom filters
    ├── Middleware/          # Custom middleware
    └── wwwroot/             # Static files
```



### Admin
- Full system access
- Manage all employees across departments
- View system-wide analytics
- Create/edit/delete employees
- Manage departments, leave types, and holidays

### Manager
- View and manage employees in their department only
- Approve/reject leave requests from department employees
- View department-specific analytics
- Submit personal leave requests

### Employee
- View personal profile and leave balances
- Submit leave requests
- View request history
- Cancel pending requests

## Key Workflows

### Creating a New Employee
1. Admin logs in
2. Navigate to Employees > Add Employee
3. Fill in employee details and select department
4. Assign role and create user credentials
5. System automatically creates leave balances for all leave types

### Submitting a Leave Request
1. Employee logs in
2. Navigate to Profile or Leave Requests
3. Select leave type, dates, and provide reason
4. System validates balance and calculates working days
5. Request is submitted for manager approval

### Approving Leave Requests
1. Manager logs in
2. Navigate to Approvals
3. View pending requests from department employees
4. Approve or reject with optional reason
5. System updates employee leave balance automatically

## Database Schema

### Main Entities
- **Employees**: Employee information and balances
- **Users**: Authentication and authorization
- **Roles**: User roles (Admin, Manager, Employee)
- **Departments**: Organizational units
- **LeaveTypes**: Types of leave available
- **LeaveRequests**: Leave request records
- **LeaveBalances**: Employee leave balances per type per year
- **Holidays**: Company holidays

## Configuration

### Leave Types
Configure in DataSeeder.cs or through admin interface:
- Annual Leave: 20 days/year
- Sick Leave: 10 days/year
- Maternity Leave: 90 days/year
- Unpaid Leave: 30 days/year
- Emergency Leave: 3 days/year

### Session Timeout
Configure in Program.cs:
```csharp
options.IdleTimeout = TimeSpan.FromMinutes(30);
```

## Logging

The application uses built-in ASP.NET Core logging:
- Log files stored in `LeaveManagement/logs/`
- Configurable log levels in `appsettings.json`
- Structured logging for all operations

## Error Handling

- Custom exception handling middleware
- User-friendly error messages
- Validation exceptions for business rule violations
- Not found exceptions for missing resources
- Automatic error logging

## Deployment

### Local Development
```bash
dotnet run --project LeaveManagement
```

### Production Deployment to Somee.com

#### Quick Deploy
```bash
deploy.bat
```
or
```bash
./deploy.ps1
```

#### Manual Deploy
1. Update `appsettings.Production.json` with Somee database credentials
2. Build the application:
```bash
dotnet publish LeaveManagement/LeaveManagement.csproj -c Release -o ./publish
```
3. Upload files from `./publish` to Somee `/wwwroot` via FTP
4. Configure .NET Core in Somee control panel
5. Test at your Somee URL

For detailed deployment instructions, see [DEPLOYMENT.md](DEPLOYMENT.md)

## Future Enhancements

- Email notifications for leave requests
- Calendar view for leave schedules
- Export reports to PDF/Excel
- Mobile application
- Multi-language support
- Leave carry-forward rules
- Delegation of approval authority

## License

This project is licensed under the MIT License.

## Support

For issues and questions, please create an issue in the repository.
