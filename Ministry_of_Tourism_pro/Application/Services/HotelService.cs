using Ministry_of_Tourism_pro.Application.DTOs;
using Ministry_of_Tourism_pro.Application.Interfaces;
using Ministry_of_Tourism_pro.Domain.Entities;
using Ministry_of_Tourism_pro.Domain.Enums;
using static Ministry_of_Tourism_pro.Application.DTOs.CreateHotelDto;

namespace Ministry_of_Tourism_pro.Application.Services
{
    public class HotelService : IHotelService
    {
        public HotelService()
        {
        }

        public async Task<IEnumerable<HotelDto>> GetAllHotelsAsync()
        {
            var hotels = new List<HotelDto>();
            string[] categories = { "5 Star", "4 Star", "3 Star", "2 Star", "1 Star", "Resort", "Lodge", "Tourist Recommended", "Not Assigned" };
            string[] hotelNames = { "Sheraton", "Sky Light", "Hilton", "Radisson Blu", "Jupiter", "Best Western", "Elilly", "Capital", "Intercontinental", "Ramada", "Marriott", "Golden Tulip", "Harmony", "Sapphire", "Nexus" };

            for (int i = 1; i <= 150; i++)
            {
                var category = categories[i % categories.Length];
                var namePrefix = hotelNames[i % hotelNames.Length];
                
                hotels.Add(new HotelDto 
                { 
                    Id = i, 
                    Name = $"{namePrefix} Addis", 
                    TradeName = $"{namePrefix} Addis",
                    RegistrationName = $"{namePrefix} International PLC",
                    DistanceFromAirport = "5 km",
                    StarCategory = i % 5 == 0 ? "5" : (i % 4 == 0 ? "4" : "3"),
                    TotalRooms = 100 + i,
                    TotalBeds = 150 + i,
                    ContactInformation = "info@hotel.com",
                    City = "Addis Ababa", 
                    Region = "Addis Ababa", 
                    Status = i % 7 == 0 ? HotelStatus.Pending : HotelStatus.Approved, 
                    NumberOfRooms = 50 + (i % 200), 
                    Category = category, 
                    TIN = $"100{i:D5}",

                    // Expanded Infrastructure
                    KingSizeRooms = 20 + i,
                    TwinBedRooms = 30 + i,
                    AllDayDining = true,
                    AllDayDiningSeats = 80 + i,
                    MeetingRoomsCount = 5 + (i % 5),
                    WifiPropertyWide = true,
                    StandbyGeneratorCapacityKva = 500,

                    RoomTypes = new List<RoomTypeDetailDto> {
                        new RoomTypeDetailDto { RoomType = "Standard Room", NumberOfRooms = 20 + (i % 50), BedType = "King Bed", MaxOccupancy = 2, PricePerNight = 2500 + (i * 10) },
                        new RoomTypeDetailDto { RoomType = "Executive Suite", NumberOfRooms = 5, BedType = "King Bed", MaxOccupancy = 3, PricePerNight = 7500 + (i * 20) }
                    },
                    Venues = new List<VenueDetailDto> {
                        new VenueDetailDto { Name = "Main Convention Hall", Type = "Conference Hall", MaxSeatingCapacity = 150 + i, StandingCapacity = 300 + i, Facilities = "Projector, WiFi, Sound System" }
                    },
                    DiningFacilities = new List<DiningDetailDto> {
                        new DiningDetailDto { Name = "International Buffet", Specialization = "Regular Restaurant", CuisineType = "Mixed", SeatingCapacity = 100 + i }
                    },
                    ImagePaths = new List<string> { $"https://picsum.photos/800/600?random={i}" }, 
                    Description = $"This is a test entry for property No. {i} to evaluate high-density data scrolling and filtering in the Commissioner Portal." 
                });
            }

            return await Task.FromResult(hotels);
        }

        public async Task<IEnumerable<HotelDto>> GetHotelsByOwnerAsync(string ownerId)
        {
            return (await GetAllHotelsAsync()).Where(h => h.Id == 1); 
        }

        public async Task<HotelDto?> GetHotelByIdAsync(int id)
        {
            return (await GetAllHotelsAsync()).FirstOrDefault(h => h.Id == id);
        }

        public async Task<int> CreateHotelAsync(CreateHotelDto dto, string ownerId)
        {
            return 1; 
        }

        public async Task UpdateHotelStatusAsync(int hotelId, HotelStatus status, string? comment = null)
        {
            await Task.CompletedTask;
        }

        public async Task UpdateHotelAsync(HotelDto dto)
        {
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<HotelDto>> GetPendingHotelsAsync()
        {
            return (await GetAllHotelsAsync()).Where(h => h.Status == HotelStatus.Pending);
        }
    }
}

