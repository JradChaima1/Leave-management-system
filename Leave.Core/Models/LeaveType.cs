namespace Leave.Core.Models
{
    public class LeaveType
    {
        public int LeaveTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxDaysPerYear { get; set; }
        public bool RequiresApproval { get; set; }
        public bool IsPaid { get; set; }

        
        public ICollection<LeaveRequest> LeaveRequests { get; set; }
        public ICollection<LeaveBalance> LeaveBalances { get; set; }
    }
}
