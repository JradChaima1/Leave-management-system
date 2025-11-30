namespace Leave.Core.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int? EmployeeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public Role Role { get; set; }
        public Employee Employee { get; set; }
    }
}
