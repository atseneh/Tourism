using Ministry_of_Tourism_pro.Application.DTOs;
using Ministry_of_Tourism_pro.Domain.Enums;

namespace Ministry_of_Tourism_pro.Application.Interfaces
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelDto>> GetAllHotelsAsync();
        Task<IEnumerable<HotelDto>> GetHotelsByOwnerAsync(string ownerId);
        Task<HotelDto?> GetHotelByIdAsync(int id);
        Task<int> CreateHotelAsync(CreateHotelDto dto, string ownerId);
        Task UpdateHotelStatusAsync(int hotelId, HotelStatus status, string? comment = null);
        Task UpdateHotelAsync(HotelDto dto);
        Task<IEnumerable<HotelDto>> GetPendingHotelsAsync();
    }
}
