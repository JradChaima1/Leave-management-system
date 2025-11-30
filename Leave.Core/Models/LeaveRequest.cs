namespace Leave.Core.Models
{
    public class LeaveRequest
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string RejectionReason { get; set; }
        public string AttachmentPath { get; set; }

        
        public Employee Employee { get; set; }
        public LeaveType LeaveType { get; set; }
        public Employee Approver { get; set; }
    }
}
