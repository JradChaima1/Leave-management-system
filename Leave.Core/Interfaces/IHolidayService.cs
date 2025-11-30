using Leave.Core.Models;

namespace Leave.Core.Interfaces
{
    public interface IHolidayService
    {
        Task<IEnumerable<Holiday>> GetAllHolidaysAsync();
        Task<Holiday> GetHolidayByIdAsync(int id);
        Task<Holiday> CreateHolidayAsync(Holiday holiday);
        Task UpdateHolidayAsync(Holiday holiday);
        Task DeleteHolidayAsync(int id);
        Task<IEnumerable<Holiday>> GetHolidaysByYearAsync(int year);
        Task<bool> IsHolidayAsync(DateTime date);
    }
}
