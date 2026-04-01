using CNET_V7_Domain.Domain.ConsigneeSchema;
using Ministry_of_Tourism_pro.Domain.Enums;
using static Ministry_of_Tourism_pro.Application.DTOs.CreateHotelDto;

namespace Ministry_of_Tourism_pro.Application.DTOs
{
    public class HotelDto
    {
        public int Id { get; set; }
        public int? ConsigneeUnitId { get; set; }
        public string? ConsigneeUnitDescription { get; set; }
        public string TradeName { get; set; } = string.Empty;
        public string Name { get => TradeName; set => TradeName = value; }
        public string? BrandName { get; set; }
        public string Description { get; set; } = string.Empty;
        public int NumberOfRooms { get; set; }
        public HotelStatus Status { get; set; }
        public string? BusinessType { get; set; }
        public DateTime? DateOfEstablishment { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? TIN { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Country { get; set; } = "Ethiopia";
        public string Region { get; set; } = string.Empty;
        public string? Zone { get; set; }
        public string City { get; set; } = string.Empty;
        public string Subcity { get; set; } = string.Empty;
        public string SubCity { get => Subcity; set => Subcity = value; }
        public string? Woreda { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone1 { get; set; } = string.Empty;
        public string? Phone2 { get; set; }
        public string? Website { get; set; }
        public string? Kebele { get; set; }
        public string? HouseNumber { get; set; }
        public string? Street { get; set; }
        public string? PoBox { get; set; }
        public string? SpecificAddress { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public string ContactEmail { get => Email; set => Email = value; }
        public string ContactPhone { get => Phone1; set => Phone1 = value; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<string> ImagePaths { get; set; } = new List<string>();
        public string? RejectionComment { get; set; }

        // --- EXCEL SPECIFIC FIELDS ---

        // Property Profile
        public string? RegistrationName { get; set; }
        public string? DistanceFromAirport { get; set; }
        public string? StarCategory { get; set; }
        public int? TotalRooms { get; set; }
        public int? TotalBeds { get; set; }
        public string? ContactInformation { get; set; }
        public string? ReservationsContact { get; set; }
        public string? SustainabilityFocalPoint { get; set; }

        // Accommodation Infrastructure
        public int? KingSizeRooms { get; set; }
        public int? TwinBedRooms { get; set; }
        public int? JuniorSuites { get; set; }
        public int? Suites { get; set; }
        public int? PresidentialSuites { get; set; }
        public int? AccessibleRooms { get; set; }

        // Food & Beverage
        public bool? AllDayDining { get; set; }
        public int? AllDayDiningSeats { get; set; }
        public string? SpecialtyRestaurants { get; set; }
        public bool? CoffeeShop { get; set; }
        public int? BarsCount { get; set; }
        public bool? NightClub { get; set; }
        public int? SouvenirShops { get; set; }
        public bool? DelegationCatering { get; set; }
        public int? DelegationCateringMaxPax { get; set; }
        public bool? RefillWaterStations { get; set; }
        public bool? VegVeganOptions { get; set; }
        public bool? NoSingleUsePlastics { get; set; }

        // Meetings & Events Specs
        public int? MeetingRoomsCount { get; set; }
        public int? LargestRoomCapacityTheatre { get; set; }
        public int? LargestRoomCapacityClassroom { get; set; }
        public int? LargestRoomCapacityBanquet { get; set; }
        public double? TotalMeetingSpaceSqm { get; set; }

        // Public Facilities
        public double? InternetBandwidthDown { get; set; }
        public double? InternetBandwidthUp { get; set; }
        public double? LobbyAreaSqm { get; set; }
        public double? GreenAreaSqm { get; set; }
        public bool? PoolAvailable { get; set; }
        public string? PoolType { get; set; }
        public bool? SpaAvailable { get; set; }
        public string? SpaGender { get; set; }
        public bool? MassageService { get; set; }
        public bool? ChildrensPlayground { get; set; }
        public bool? ChildrenDayCare { get; set; }
        public bool? StaffCanteen { get; set; }

        // Accessibility
        public bool? WheelchairRamps { get; set; }
        public int? ElevatorsCount { get; set; }
        public bool? ElevatorsWheelchairSized { get; set; }
        public bool? PublicAccessibleBathroom { get; set; }

        // Security & Safety
        public bool? CCTVPublicAreas { get; set; }
        public DateTime? FireExtinguishersLastInspection { get; set; }
        public bool? HoseReels { get; set; }
        public bool? SmokeDetectorsInRooms { get; set; }
        public bool? SmokeDetectorsInPublicAreas { get; set; }
        public string? SprinklerCoverage { get; set; }
        public bool? FireAlarmControlPanel { get; set; }
        public int? EmergencyExitsCount { get; set; }
        public bool? BagScanner { get; set; }
        public bool? WalkThroughScanner { get; set; }
        public bool? HandScanner { get; set; }

        // Transport & Parking
        public int? ParkingSpacesCount { get; set; }
        public int? BusParkingCount { get; set; }
        public bool? ShuttleToCop { get; set; }
        public bool? ShuttleToAirport { get; set; }
        public bool? PublicTransportWithin500m { get; set; }
        public int? EvChargingPoints { get; set; }
        public string? EvChargerTypes { get; set; }

        // ICT & Guest Services
        public bool? WifiPropertyWide { get; set; }
        public double? WifiAvgSpeed { get; set; }
        public bool? InHouseLaundry { get; set; }
        public bool? Reception24hr { get; set; }
        public bool? VipCheckIn { get; set; }
        public bool? PassportScanner { get; set; }
        public bool? CurrencyScanner { get; set; }

        // Utilities & Resilience
        public double? StandbyGeneratorCapacityKva { get; set; }
        public string? StandbyGeneratorCoverage { get; set; }
        public string? WaterTreatment { get; set; }
        public bool? WasteSegregation { get; set; }
        public bool? Recycling { get; set; }
        public bool? HazardousWasteHandling { get; set; }

        // Sustainability & Certifications
        public string? SustainabilityCertification { get; set; }
        public string? OtherEcoLabels { get; set; }
        public bool? FoodWasteProgram { get; set; }
        public bool? SustainabilityRefillWaterStations { get; set; }

        // Staffing & Languages
        public int? TotalStaff { get; set; }
        public string? InternationalLanguagesFrontDesk { get; set; }

        // Internal Data Storage
        public List<IdentificationDTO> RawIdentifications { get; set; } = new();

        // Legacy/Existing Infrastructure (keeping for compatibility)
        public List<RoomTypeDetailDto> RoomTypes { get; set; } = new List<RoomTypeDetailDto>();
        public List<VenueDetailDto> Venues { get; set; } = new List<VenueDetailDto>();
        public List<DiningDetailDto> DiningFacilities { get; set; } = new List<DiningDetailDto>();
    }

    public class CreateHotelDto
    {
        // Basic Information
        public string TradeName { get; set; } = string.Empty;
        public string? BrandName { get; set; }
        public string? BusinessType { get; set; }
        public DateTime? DateOfEstablishment { get; set; }
        public bool IsActive { get; set; } = true;
        public string Code { get; set; } = string.Empty;
        public string? TIN { get; set; }
        public string Category { get; set; } = string.Empty;
        public int NumberOfRooms { get; set; }

        // Contact Information
        public string Phone1 { get; set; } = string.Empty;
        public string? Phone2 { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Website { get; set; }

        // Address Details
        public string? SpecificAddress { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? PoBox { get; set; }
        public string Country { get; set; } = "Ethiopia";
        public string Region { get; set; } = string.Empty;
        public string? Zone { get; set; }
        public string? Woreda { get; set; }
        public string City { get; set; } = string.Empty;
        public string? Subcity { get; set; }
        public string? Kebele { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }

        // Location Info
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // All Excel specific fields should also be in CreateHotelDto (simplified for brevity here, but usually identical to HotelDto)
        // [Add matching fields from HotelDto here if necessary for creation flow]

        // Legacy fields (optional compatibility)
        public string Name { get => TradeName; set => TradeName = value; }
        public string ContactEmail { get => Email; set => Email = value; }
        public string ContactPhone { get => Phone1; set => Phone1 = value; }
        public string? SubCity { get => Subcity; set => Subcity = value; }
        public class RoomTypeDetailDto
        {
            public string RoomType { get; set; } = string.Empty; // Single, Double, etc.
            public int NumberOfRooms { get; set; }
            public string BedType { get; set; } = string.Empty; // Single, Double, King, etc.
            public int MaxOccupancy { get; set; }
            public string RoomSize { get; set; } = string.Empty;
            public decimal PricePerNight { get; set; }
            public string Amenities { get; set; } = string.Empty; // WiFi, TV, AC, etc.
        }

        public class VenueDetailDto
        {
            public string Name { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty; // Conference Hall, Meeting Room, etc.
            public string FacilityName { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int MaxSeatingCapacity { get; set; }
            public int StandingCapacity { get; set; }
            public string TableArrangements { get; set; } = string.Empty; // Theater, Classroom, etc.
            public string Facilities { get; set; } = string.Empty; // Projector, Mic, etc.
            public bool ParkingAvailability { get; set; }
            public bool Accessibility { get; set; }
            public double AreaSize { get; set; }
            public double CeilingHeight { get; set; }
        }

        public class DiningDetailDto
        {
            public string Name { get; set; } = string.Empty;
            public string Specialization { get; set; } = string.Empty; // Catering, Restaurant, Both
            public string CuisineType { get; set; } = string.Empty;
            public int SeatingCapacity { get; set; }
            public string OperatingHours { get; set; } = string.Empty;
            public string DiningSetting { get; set; } = string.Empty; // Indoor, Outdoor
            public string SpecialServices { get; set; } = string.Empty;

            // Catering specific
            public bool EventCateringAvailable { get; set; }
            public int MaxCateringCapacity { get; set; }
            public string MenuOptions { get; set; } = string.Empty;
            public string ServiceTypes { get; set; } = string.Empty; // Buffet, Set Menu, etc.
        }
    }
}

