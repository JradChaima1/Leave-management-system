namespace Leave.Core.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ManagerId { get; set; }

        public Employee Manager { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
