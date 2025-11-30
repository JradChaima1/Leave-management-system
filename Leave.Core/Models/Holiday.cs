namespace Leave.Core.Models
{
    public class Holiday
    {
        public int HolidayId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsRecurring { get; set; }
        public string Description { get; set; }
    }
}
