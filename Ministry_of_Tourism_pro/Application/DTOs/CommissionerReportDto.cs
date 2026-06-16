namespace Ministry_of_Tourism_pro.Application.DTOs
{
    public class CommissionerReportDto
    {
        public List<GeneralReportItem> GeneralRegistry { get; set; } = new();
        public List<AccommodationReportItem> AccommodationInfrastructure { get; set; } = new();
        public List<DiningReportItem> FoodAndBeverage { get; set; } = new();
        public List<MeetingEventReportItem> MeetingsEvents { get; set; } = new();
        public List<RatingSummaryItem> RatingSummary { get; set; } = new();

        // Parking report data
        public List<ParkingReportItem> ParkingCapacity { get; set; } = new();

        // New report data for the 8 missing reports
        public List<CertifiedFacilityReportItem> CertifiedFacilities { get; set; } = new();
        public List<MiceDestinationReportItem> MiceDestinations { get; set; } = new();
        public List<EventVenueReportItem> EventVenues { get; set; } = new();
        public List<KitchenPosReportItem> KitchenPosSystems { get; set; } = new();
        public List<RestaurantSeatingReportItem> RestaurantSeating { get; set; } = new();
        public List<BarsLoungeReportItem> BarsLounge { get; set; } = new();
        public List<AccessibilityReportItem> AccessibilityFacilities { get; set; } = new();
        public List<PppAnalyticsReportItem> PppAnalytics { get; set; } = new();
    }

    public class GeneralReportItem
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string TIN { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalRooms { get; set; }
        public int TotalBeds { get; set; }
        public int TotalUnits { get; set; }
        public int TotalSpaces { get; set; }
        public string StarRating { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public string SpecificAddress { get; set; } = string.Empty;
    }

    public class AccommodationReportItem
    {
        public string PropertyName { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public string BedConfig { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Price { get; set; }
        public int MaxPax { get; set; }
    }

    public class DiningReportItem
    {
        public string PropertyName { get; set; } = string.Empty;
        public string FacilityName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Cuisine { get; set; } = string.Empty;
        public int Seating { get; set; }
        public bool CateringAvailable { get; set; }
    }

    public class MeetingEventReportItem
    {
        public string PropertyName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int SeatingCapacity { get; set; }
        public int StandingCapacity { get; set; }
        public double AreaSqm { get; set; }
    }

    public class RatingSummaryItem
    {
        public string Category { get; set; } = string.Empty;
        public int PropertyCount { get; set; }
        public int TotalRooms { get; set; }
        public int TotalBeds { get; set; }
        public double AvgRoomsPerProperty { get; set; }
    }

    // --- New Report DTOs ---

    public class CertifiedFacilityReportItem
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StarRating { get; set; } = string.Empty;
        public string TourismLicense { get; set; } = string.Empty;
        public string FireSafetyCert { get; set; } = string.Empty;
        public string EnvironmentalClearance { get; set; } = string.Empty;
        public string FoodSafetyCert { get; set; } = string.Empty;
        public string IsoCertification { get; set; } = string.Empty;
        public string CertificationStatus { get; set; } = string.Empty;
    }

    public class MiceDestinationReportItem
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StarRating { get; set; } = string.Empty;
        public int MeetingRoomsCount { get; set; }
        public int LargestCapacity { get; set; }
        public double TotalMeetingSpaceSqm { get; set; }
        public bool WifiAvailable { get; set; }
        public bool GeneratorAvailable { get; set; }
        public string MiceScore { get; set; } = string.Empty;
    }

    public class EventVenueReportItem
    {
        public string PropertyName { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string SettingArrangement { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal CeilingHeight { get; set; }
        public double AreaSqm { get; set; }
    }

    public class KitchenPosReportItem
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool OnlineOrderingSystem { get; set; }
        public bool TableReservation { get; set; }
        public bool IpTv { get; set; }
        public bool WifiPropertyWide { get; set; }
        public int RestaurantsCount { get; set; }
        public int MeetingRoomsCount { get; set; }
        public string IntegrationStatus { get; set; } = string.Empty;
    }

    public class RestaurantSeatingReportItem
    {
        public string PropertyName { get; set; } = string.Empty;
        public string RestaurantName { get; set; } = string.Empty;
        public string CuisineType { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsHalal { get; set; }
        public bool IsVegan { get; set; }
        public int AllDayDiningSeats { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class BarsLoungeReportItem
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StarRating { get; set; } = string.Empty;
        public int BarsCount { get; set; }
        public bool NightClub { get; set; }
        public bool CoffeeShop { get; set; }
        public bool DelegationCatering { get; set; }
        public int DelegationCateringMaxPax { get; set; }
        public bool VegVeganOptions { get; set; }
        public bool RefillWaterStations { get; set; }
    }

    public class AccessibilityReportItem
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StarRating { get; set; } = string.Empty;
        public bool WheelchairRamps { get; set; }
        public int ElevatorsCount { get; set; }
        public bool ElevatorsWheelchairSized { get; set; }
        public bool PublicAccessibleBathroom { get; set; }
        public bool SpaAvailable { get; set; }
        public bool ChildrensPlayground { get; set; }
        public bool ChildrenDayCare { get; set; }
    }

    public class ParkingReportItem
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public int TotalSlots { get; set; }
        public int BusParkingCount { get; set; }
        public bool ValetParking { get; set; }
        public bool ParkingWithin100m { get; set; }
        public int EvChargingPoints { get; set; }
        public string EvChargerTypes { get; set; } = string.Empty;
    }

    public class PppAnalyticsReportItem
    {
        public int Id { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StarRating { get; set; } = string.Empty;
        public int TotalRooms { get; set; }
        public int TotalBeds { get; set; }
        public int MeetingRooms { get; set; }
        public bool WifiAvailable { get; set; }
        public bool GeneratorAvailable { get; set; }
        public bool SustainabilityCertified { get; set; }
        public string SustainabilityCertification { get; set; } = string.Empty;
        public int TotalStaff { get; set; }
        public string PppStatus { get; set; } = string.Empty;
    }
}
