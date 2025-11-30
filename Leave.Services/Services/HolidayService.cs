using Leave.Core.Models;
using Leave.Core.Interfaces;

namespace Leave.Services.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly IRepository<Holiday> _holidayRepository;

        public HolidayService(IRepository<Holiday> holidayRepository)
        {
            _holidayRepository = holidayRepository;
        }

        public async Task<IEnumerable<Holiday>> GetAllHolidaysAsync()
        {
            return await _holidayRepository.GetAllAsync();
        }

        public async Task<Holiday> GetHolidayByIdAsync(int id)
        {
            var holiday = await _holidayRepository.GetByIdAsync(id);
            if (holiday == null)
                throw new Exception($"Holiday with ID {id} not found");
            return holiday;
        }

        public async Task<Holiday> CreateHolidayAsync(Holiday holiday)
        {
            if (string.IsNullOrEmpty(holiday.Name))
                throw new Exception("Holiday name is required");

            return await _holidayRepository.AddAsync(holiday);
        }

        public async Task UpdateHolidayAsync(Holiday holiday)
        {
            var existing = await _holidayRepository.GetByIdAsync(holiday.HolidayId);
            if (existing == null)
                throw new Exception($"Holiday with ID {holiday.HolidayId} not found");

            await _holidayRepository.UpdateAsync(holiday);
        }

        public async Task DeleteHolidayAsync(int id)
        {
            var holiday = await _holidayRepository.GetByIdAsync(id);
            if (holiday == null)
                throw new Exception($"Holiday with ID {id} not found");

            await _holidayRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Holiday>> GetHolidaysByYearAsync(int year)
        {
            var holidays = await _holidayRepository.GetAllAsync();
            return holidays.Where(h => h.Date.Year == year);
        }

        public async Task<bool> IsHolidayAsync(DateTime date)
        {
            var holidays = await _holidayRepository.GetAllAsync();
            return holidays.Any(h => h.Date.Date == date.Date);
        }
    }
}
