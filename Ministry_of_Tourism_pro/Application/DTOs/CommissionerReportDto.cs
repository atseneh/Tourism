namespace Ministry_of_Tourism_pro.Application.DTOs
{
    public class CommissionerReportDto
    {
        public List<GeneralReportItem> GeneralRegistry { get; set; } = new();
        public List<AccommodationReportItem> AccommodationInfrastructure { get; set; } = new();
        public List<DiningReportItem> FoodAndBeverage { get; set; } = new();
        public List<MeetingEventReportItem> MeetingsEvents { get; set; } = new();
        public List<RatingSummaryItem> RatingSummary { get; set; } = new();
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
        public int TotalRooms { get; set; }
        public int TotalUnits { get; set; }
        public int TotalSpaces { get; set; }
        public string StarRating { get; set; } = string.Empty;
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
        public double AvgRoomsPerProperty { get; set; }
    }
}
