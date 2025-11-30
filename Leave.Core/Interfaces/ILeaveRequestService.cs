using Leave.Core.Models;

namespace Leave.Core.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequest>> GetAllRequestsAsync();
        Task<LeaveRequest> GetRequestByIdAsync(int id);
        Task<LeaveRequest> SubmitLeaveRequestAsync(LeaveRequest request);
        Task ApproveRequestAsync(int requestId, int approvedBy);
        Task RejectRequestAsync(int requestId, int rejectedBy, string reason);
        Task CancelRequestAsync(int requestId);
        Task<IEnumerable<LeaveRequest>> GetEmployeeRequestsAsync(int employeeId);
        Task<IEnumerable<LeaveRequest>> GetPendingRequestsForManagerAsync(int managerId);
        Task<bool> ValidateLeaveRequestAsync(LeaveRequest request);
    }
}
