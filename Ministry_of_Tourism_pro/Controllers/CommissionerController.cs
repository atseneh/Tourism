using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.aatmSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ministry_of_Tourism_pro.Application.DTOs;
using Ministry_of_Tourism_pro.Application.Interfaces;
using Ministry_of_Tourism_pro.Common;
using Ministry_of_Tourism_pro.Domain.Enums;
using Newtonsoft.Json;

namespace Ministry_of_Tourism_pro.Controllers
{
    [Authorize(Roles = "Commissioner")]
    public class CommissionerController : Controller
    {
        private readonly IHotelService _hotelService;
        private readonly SharedHelpers _sharedHelpers;

        public CommissionerController(IHotelService hotelService, SharedHelpers sharedHelpers)
        {
            _hotelService = hotelService;
            _sharedHelpers = sharedHelpers;
        }

        public async Task<IActionResult> Overview()
        {
            var hotels = await _hotelService.GetAllHotelsAsync();
            return View(hotels);
        }

        public async Task<IActionResult> Review(int id)
        {
            var parameters = new Dictionary<string, string>
            {
                { "code", id.ToString() }
            };
            var consignees = await _sharedHelpers.GetFilterDynamic<List<CNET_V7_Domain.Domain.ViewSchema.VwConsigneeViewDTO>>("VwConsigneeView", parameters);
            var consignee = consignees?.FirstOrDefault();
            
            if (consignee == null) return NotFound();
            return View(consignee);
        }

        public async Task<IActionResult> PendingApprovals()
        {
            var parameters = new Dictionary<string, string>
            {
                //{ "childpreferenceID", "62" },
                // { "[consigneeIsActive]", "true" },
                { "gslType", "28" },
            };

            // Using GetFilterDynamic for view-based data
            var consignees = await _sharedHelpers.GetFilterDynamic<List<CNET_V7_Domain.Domain.ViewSchema.VwConsigneeViewDTO>>("VwConsigneeView", parameters);
            
            var approvalList = new List<Ministry_of_Tourism_pro.Application.DTOs.ApprovalQueueItemDto>();

            if (consignees != null)
            {
                foreach (var c in consignees.Take(20)) // Limit for performance
                {
                    var identifications = await _sharedHelpers.GetFilterData<List<IdentificationDTO>>("Identification", new Dictionary<string, string> { { "consignee", c.Id.ToString() } });
                    
                    approvalList.Add(new Ministry_of_Tourism_pro.Application.DTOs.ApprovalQueueItemDto
                    {
                        Id = (int)c.Id,
                        PropertyName = c.FirstName ?? "N/A",
                        Tin = c.Tin ?? "N/A",
                        Code = c.Code ?? "N/A",
                        Subcity = c.SubCityName ?? "N/A",
                        SpecificAddress = identifications?.FirstOrDefault(x => x.Description == "SpecificAddress")?.IdNumber ?? "N/A",
                        StarRating = identifications?.FirstOrDefault(x => x.Description == "StarCategory")?.IdNumber ?? "N/A",
                        IsActive = c.ConsigneeIsActive,
                        AddressLine1 = c.AddressLine1 ?? "",
                        PreferenceDescription = c.ChildPreferenceDescrption ?? ""
                    });
                }
            }
            
            return View(approvalList);
        }

        public async Task<IActionResult> Reports()
        {
            // Return mocks initially as requested
            var hotels = await _hotelService.GetAllHotelsAsync();
            return View(hotels);
        }

        [HttpGet]
        public async Task<IActionResult> GetRealReportData(string? id = null)
        {
            try 
            {
                var report = new CommissionerReportDto();
                
                // 1. Fetch establishments from VwConsigneeView
                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(id))
                {
                    parameters.Add("id", id);
                }
                else
                {
                    parameters.Add("gslType", "28");
                }

                var data = await _sharedHelpers.GetFilterDynamic<List<CNET_V7_Domain.Domain.ViewSchema.VwConsigneeViewDTO>>("VwConsigneeView", parameters);
                
                if (data == null || !data.Any())
                {
                    return Json(report);
                }

                foreach (var c in data)
                {
                    var category = c.ChildPreferenceDescrption ?? "General Sector";
                    var tradeName = c.FirstName ?? "Unnamed Establishment";
                    var starRating = c.NationalId ?? string.Empty;
                    if (!string.IsNullOrEmpty(starRating) && char.IsDigit(starRating[0]) && !starRating.Contains("Star"))
                        starRating = starRating + " Star";

                    // 2. Fetch InfrastructureProfile (new structure)
                    HotelInfrastructureProfileDTO? profile = null;
                    if (c.ConsigneeUnitId.HasValue)
                    {
                        var profiles = await _sharedHelpers.GetFilterData<List<HotelInfrastructureProfileDTO>>("HotelInfrastructureProfile", new Dictionary<string, string>
                        {
                            { "consigneeId", c.Id.ToString() },
                            { "consigneeUnitId", c.ConsigneeUnitId.Value.ToString() }
                        });
                        profile = profiles?.FirstOrDefault();
                    }

                    // 3. Fetch FacilityList (new structure)
                    List<HotelFacilityListDTO>? facilities = null;
                    if (c.ConsigneeUnitId.HasValue)
                    {
                        facilities = await _sharedHelpers.GetFilterData<List<HotelFacilityListDTO>>("HotelFacilityList", new Dictionary<string, string>
                        {
                            { "consigneeId", c.Id.ToString() },
                            { "consigneeUnitId", c.ConsigneeUnitId.Value.ToString() }
                        });
                    }

                    // Calculate totals from profile
                    int totalRooms = (profile?.KingSizeRooms ?? 0) + (profile?.TwinBedRooms ?? 0) + (profile?.JuniorSuites ?? 0) +
                                     (profile?.Suites ?? 0) + (profile?.PresidentialSuites ?? 0) + (profile?.AccessibleRooms ?? 0);
                    int totalBeds = (profile?.KingSizeRooms ?? 0) +
                                    ((profile?.TwinBedRooms ?? 0) * 2) +
                                    (profile?.JuniorSuites ?? 0) +
                                    ((profile?.Suites ?? 0) * 3) +
                                    ((profile?.PresidentialSuites ?? 0) * 3) +
                                    (profile?.AccessibleRooms ?? 0);

                    // Calculate meeting aggregates from facility list
                    int meetingRoomsCount = 0;
                    int maxTheater = 0;
                    double totalMeetingSqm = 0;
                    var meetingFacilities = facilities?.Where(f => f.FacilityTypeConstantId == 3).ToList() ?? new List<HotelFacilityListDTO>();
                    meetingRoomsCount = meetingFacilities.Count;
                    foreach (var mf in meetingFacilities)
                    {
                        double w = (double)(mf.Width ?? 0);
                        double l = (double)(mf.Length ?? 0);
                        totalMeetingSqm += (w * l);
                        int cap = mf.Capacity ?? 0;
                        if (cap > maxTheater) maxTheater = cap;
                    }

                    int restaurantCount = facilities?.Count(f => f.FacilityTypeConstantId == 1) ?? 0;

                    // ========== A. General Registry ==========
                    report.GeneralRegistry.Add(new GeneralReportItem
                    {
                        Id = c.Id,
                        PropertyName = tradeName,
                        Category = category,
                        City = c.SubCityName ?? "Addis Ababa",
                        Region = c.CityName ?? "Addis Ababa",
                        TIN = c.Tin ?? "N/A",
                        Phone = c.Phone1 ?? "N/A",
                        Email = c.Email ?? string.Empty,
                        TotalRooms = totalRooms,
                        TotalBeds = totalBeds,
                        TotalUnits = c.ConsigneeUnitId.HasValue ? 1 : 0,
                        TotalSpaces = meetingRoomsCount,
                        StarRating = starRating,
                        ManagerName = c.AddressLine2 ?? string.Empty,
                        SpecificAddress = c.SpecificAddress ?? string.Empty
                    });

                    // ========== B. Accommodation Infrastructure ==========
                    var roomTypes = new Dictionary<string, string> {
                        { "KingSizeRooms", "King Size" },
                        { "TwinBedRooms", "Twin Bed" },
                        { "JuniorSuites", "Junior Suite" },
                        { "Suites", "Suite" },
                        { "PresidentialSuites", "Presidential" },
                        { "AccessibleRooms", "Accessible" }
                    };
                    foreach (var rt in roomTypes)
                    {
                        if (profile == null) continue;
                        var prop = typeof(HotelInfrastructureProfileDTO).GetProperty(rt.Key);
                        var val = prop?.GetValue(profile) as int? ?? 0;
                        if (val > 0)
                        {
                            report.AccommodationInfrastructure.Add(new AccommodationReportItem
                            {
                                PropertyName = tradeName,
                                RoomType = rt.Value,
                                BedConfig = rt.Value.Contains("Twin") ? "Twin" : "Large/King",
                                Count = val,
                                Price = 0,
                                MaxPax = rt.Value.Contains("Suite") || rt.Value.Contains("Presidential") ? 3 : 2
                            });
                        }
                    }

                    // ========== C. Food & Beverage (from FacilityList type=1 + Profile) ==========
                    if (facilities != null)
                    {
                        foreach (var f in facilities.Where(x => x.FacilityTypeConstantId == 1))
                        {
                            report.FoodAndBeverage.Add(new DiningReportItem
                            {
                                PropertyName = tradeName,
                                FacilityName = f.Name ?? "Restaurant",
                                Specialization = f.Remark ?? "Dining",
                                Cuisine = f.Cuisine ?? "International",
                                Seating = f.Capacity ?? 0,
                                CateringAvailable = profile?.DelegationCatering ?? false
                            });
                        }
                    }
                    // Add AllDayDining if present
                    if (profile?.AllDayDining == true && (profile?.AllDayDiningSeats ?? 0) > 0)
                    {
                        report.FoodAndBeverage.Add(new DiningReportItem
                        {
                            PropertyName = tradeName,
                            FacilityName = "All-Day Dining",
                            Specialization = "Full Service",
                            Cuisine = "International",
                            Seating = profile.AllDayDiningSeats ?? 0,
                            CateringAvailable = profile?.DelegationCatering ?? false
                        });
                    }

                    // ========== D. Meetings & Events (from FacilityList type=3) ==========
                    foreach (var mf in meetingFacilities)
                    {
                        report.MeetingsEvents.Add(new MeetingEventReportItem
                        {
                            PropertyName = tradeName,
                            VenueName = mf.Name ?? "Meeting Room",
                            Type = mf.Remark ?? "Meeting",
                            SeatingCapacity = mf.Capacity ?? 0,
                            StandingCapacity = (int)((double)(mf.Width ?? 0) * (double)(mf.Length ?? 0) / 1.5), // heuristic standing
                            AreaSqm = (double)(mf.Width ?? 0) * (double)(mf.Length ?? 0)
                        });
                    }

                    // ========== E. Rating Summary (grouped by star) ==========
                    report.RatingSummary = report.GeneralRegistry
                        .Select(x => {
                            var rating = x.StarRating;
                            if (string.IsNullOrEmpty(rating) || rating == "0 Star")
                                rating = x.Category.Contains("Star") ? x.Category : "Not Assigned";
                            return new { x.TotalRooms, x.TotalBeds, Rating = rating };
                        })
                        .GroupBy(x => x.Rating)
                        .Select(g => new RatingSummaryItem
                        {
                            Category = g.Key,
                            PropertyCount = g.Count(),
                            TotalRooms = g.Sum(x => x.TotalRooms),
                            TotalBeds = g.Sum(x => x.TotalBeds),
                            AvgRoomsPerProperty = g.Any() ? Math.Round(g.Average(x => x.TotalRooms), 1) : 0
                        })
                        .OrderByDescending(x => x.Category == "Not Assigned" ? "0" : x.Category)
                        .ToList();

                    // ========== F. Certified Facilities ==========
                    report.CertifiedFacilities.Add(new CertifiedFacilityReportItem
                    {
                        Id = c.Id,
                        PropertyName = tradeName,
                        Category = category,
                        City = c.SubCityName ?? "Addis Ababa",
                        StarRating = starRating,
                        TourismLicense = profile?.TourismServiceCompetenceLicenseCertificate ?? "N/A",
                        FireSafetyCert = profile?.FireSafetyCertificate ?? "N/A",
                        EnvironmentalClearance = profile?.EnvironmentalClearanceCertificate ?? "N/A",
                        FoodSafetyCert = profile?.FoodSafetyAndHygieneCertificate ?? "N/A",
                        IsoCertification = profile?.IsoCertification ?? "N/A",
                        CertificationStatus = GetCertificationStatus(profile)
                    });

                    // ========== G. MICE Destinations ==========
                    string miceScore = "Low";
                    if (meetingRoomsCount >= 3 && maxTheater >= 100 && (profile?.WifiPropertyWide == true))
                        miceScore = "High";
                    else if (meetingRoomsCount >= 1 || maxTheater >= 30)
                        miceScore = "Medium";

                    report.MiceDestinations.Add(new MiceDestinationReportItem
                    {
                        Id = c.Id,
                        PropertyName = tradeName,
                        Category = category,
                        City = c.SubCityName ?? "Addis Ababa",
                        StarRating = starRating,
                        MeetingRoomsCount = meetingRoomsCount,
                        LargestCapacity = maxTheater,
                        TotalMeetingSpaceSqm = totalMeetingSqm,
                        WifiAvailable = profile?.WifiPropertyWide == true,
                        GeneratorAvailable = (profile?.StandbyGeneratorCapacityKva ?? 0) > 0,
                        MiceScore = miceScore
                    });

                    // ========== H. Event Venues (from FacilityList type=3) ==========
                    foreach (var mf in meetingFacilities)
                    {
                        report.EventVenues.Add(new EventVenueReportItem
                        {
                            PropertyName = tradeName,
                            VenueName = mf.Name ?? "Venue",
                            Type = mf.Remark ?? "Conference Hall",
                            SettingArrangement = mf.SettingArrangement ?? "Theatre",
                            Capacity = mf.Capacity ?? 0,
                            Width = mf.Width ?? 0,
                            Length = mf.Length ?? 0,
                            CeilingHeight = mf.CeilingHeight ?? 0,
                            AreaSqm = (double)((mf.Width ?? 0) * (mf.Length ?? 0))
                        });
                    }

                    // ========== I. Kitchen & POS Technology ==========
                    string integrationStatus = "Basic";
                    int techScore = 0;
                    if (profile?.WifiPropertyWide == true) techScore++;
                    if (profile?.OnlineOrderingSystem == true) techScore++;
                    if (profile?.OnlineBookingSystem == true) techScore++;
                    if (profile?.TableReservation == true) techScore++;
                    if (profile?.IpTv == true) techScore++;
                    if (techScore >= 4) integrationStatus = "Fully Integrated";
                    else if (techScore >= 2) integrationStatus = "Partially Integrated";

                    report.KitchenPosSystems.Add(new KitchenPosReportItem
                    {
                        Id = c.Id,
                        PropertyName = tradeName,
                        City = c.SubCityName ?? "Addis Ababa",
                        OnlineOrderingSystem = profile?.OnlineOrderingSystem == true,
                        TableReservation = profile?.TableReservation == true,
                        IpTv = profile?.IpTv == true,
                        WifiPropertyWide = profile?.WifiPropertyWide == true,
                        RestaurantsCount = restaurantCount,
                        MeetingRoomsCount = meetingRoomsCount,
                        IntegrationStatus = integrationStatus
                    });

                    // ========== J. Restaurant Seating (from FacilityList type=1) ==========
                    if (facilities != null)
                    {
                        foreach (var f in facilities.Where(x => x.FacilityTypeConstantId == 1))
                        {
                            report.RestaurantSeating.Add(new RestaurantSeatingReportItem
                            {
                                PropertyName = tradeName,
                                RestaurantName = f.Name ?? "Restaurant",
                                CuisineType = f.Cuisine ?? "International",
                                Type = f.Remark ?? "Restaurant",
                                Capacity = f.Capacity ?? 0,
                                IsHalal = f.IsHalal == true,
                                IsVegan = f.IsVegan == true,
                                AllDayDiningSeats = profile?.AllDayDiningSeats ?? 0,
                                Notes = f.Notes ?? string.Empty
                            });
                        }
                    }

                    // ========== K. Bars & Lounge ==========
                    report.BarsLounge.Add(new BarsLoungeReportItem
                    {
                        Id = c.Id,
                        PropertyName = tradeName,
                        City = c.SubCityName ?? "Addis Ababa",
                        StarRating = starRating,
                        BarsCount = profile?.BarsCount ?? 0,
                        NightClub = profile?.NightClub == true,
                        CoffeeShop = profile?.CoffeeShop == true,
                        DelegationCatering = profile?.DelegationCatering == true,
                        DelegationCateringMaxPax = profile?.DelegationCateringMaxPax ?? 0,
                        VegVeganOptions = profile?.VegVeganOptions == true,
                        RefillWaterStations = profile?.RefillWaterStations == true
                    });

                    // ========== L. Accessibility ==========
                    report.AccessibilityFacilities.Add(new AccessibilityReportItem
                    {
                        Id = c.Id,
                        PropertyName = tradeName,
                        City = c.SubCityName ?? "Addis Ababa",
                        StarRating = starRating,
                        WheelchairRamps = profile?.WheelchairRamps == true,
                        ElevatorsCount = profile?.ElevatorsCount ?? 0,
                        ElevatorsWheelchairSized = profile?.ElevatorsWheelchairSized == true,
                        PublicAccessibleBathroom = profile?.PublicAccessibleBathroom == true,
                        SpaAvailable = profile?.SpaAvailable == true,
                        ChildrensPlayground = profile?.ChildrensPlayground == true,
                        ChildrenDayCare = profile?.ChildrenDayCare == true
                    });

                    // ========== M. Parking Capacity ==========
                    report.ParkingCapacity.Add(new ParkingReportItem
                    {
                        Id = c.Id,
                        PropertyName = tradeName,
                        TotalSlots = profile?.ParkingSpacesCount ?? 0,
                        BusParkingCount = profile?.BusParkingCount ?? 0,
                        ValetParking = profile?.ValetParking == true,
                        ParkingWithin100m = profile?.ParkingWithin100m == true,
                        EvChargingPoints = profile?.EvChargingPoints ?? 0,
                        EvChargerTypes = profile?.EvChargerTypes ?? "N/A"
                    });

                    // ========== N. PPP Analytics ==========
                    string pppStatus = "Standard";
                    if ((profile?.WifiPropertyWide ?? false) && (profile?.SustainabilityCertification ?? "N/A") != "N/A" && totalRooms > 50)
                        pppStatus = "PPP Ready";
                    else if (totalRooms > 20 || meetingRoomsCount > 0)
                        pppStatus = "Potential PPP";

                    report.PppAnalytics.Add(new PppAnalyticsReportItem
                    {
                        Id = c.Id,
                        PropertyName = tradeName,
                        Category = category,
                        City = c.SubCityName ?? "Addis Ababa",
                        StarRating = starRating,
                        TotalRooms = totalRooms,
                        TotalBeds = totalBeds,
                        MeetingRooms = meetingRoomsCount,
                        WifiAvailable = profile?.WifiPropertyWide == true,
                        GeneratorAvailable = (profile?.StandbyGeneratorCapacityKva ?? 0) > 0,
                        SustainabilityCertified = !string.IsNullOrEmpty(profile?.SustainabilityCertification) && profile?.SustainabilityCertification != "N/A",
                        SustainabilityCertification = profile?.SustainabilityCertification ?? "N/A",
                        TotalStaff = (profile?.LineStaff ?? 0) + (profile?.ManagementStaff ?? 0),
                        PppStatus = pppStatus
                    });
                }

                return Json(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private string GetCertificationStatus(HotelInfrastructureProfileDTO? profile)
        {
            if (profile == null) return "No Data";
            int certCount = 0;
            if (!string.IsNullOrEmpty(profile.TourismServiceCompetenceLicenseCertificate) && profile.TourismServiceCompetenceLicenseCertificate != "N/A") certCount++;
            if (!string.IsNullOrEmpty(profile.FireSafetyCertificate) && profile.FireSafetyCertificate != "N/A") certCount++;
            if (!string.IsNullOrEmpty(profile.EnvironmentalClearanceCertificate) && profile.EnvironmentalClearanceCertificate != "N/A") certCount++;
            if (!string.IsNullOrEmpty(profile.FoodSafetyAndHygieneCertificate) && profile.FoodSafetyAndHygieneCertificate != "N/A") certCount++;
            if (!string.IsNullOrEmpty(profile.IsoCertification) && profile.IsoCertification != "N/A") certCount++;

            if (certCount >= 4) return "Fully Certified";
            if (certCount >= 2) return "Partially Certified";
            if (certCount >= 1) return "Minimally Certified";
            return "Uncertified";
        }

        [HttpGet]
        public async Task<IActionResult> GetEstablishmentIdentifications(int id)
        {
            try
            {
                // Fetch all identifications for the given consignee
                var identifications = await _sharedHelpers.GetFilterData<List<IdentificationDTO>>("Identification", new Dictionary<string, string> 
                { 
                    { "consignee", id.ToString() }
                });

                return Json(identifications ?? new List<IdentificationDTO>());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailPartial(int id)
        {
            try
            {
                // 1. Fetch Establishment from View
                var parameters = new Dictionary<string, string> { { "id", id.ToString() } };
                var consignees = await _sharedHelpers.GetFilterDynamic<List<CNET_V7_Domain.Domain.ViewSchema.VwConsigneeViewDTO>>("VwConsigneeView", parameters);
                var c = consignees?.FirstOrDefault();
                
                if (c == null) return NotFound();

                // 2. Fetch Identifications
                var identifications = await _sharedHelpers.GetFilterData<List<IdentificationDTO>>("Identification", new Dictionary<string, string> 
                { 
                    { "consignee", id.ToString() }
                }) ?? new List<IdentificationDTO>();

                // 3. Map to HotelDto
                var hotel = new HotelDto
                {
                    Id = (int)c.Id,
                    Name = c.FirstName ?? "N/A",
                    TradeName = c.FirstName ?? "N/A",
                    RegistrationName = c.FirstName ?? "N/A",
                    TIN = c.Tin ?? "N/A",
                    Code = c.Code ?? "N/A",
                    City = c.CityName ?? "Addis Ababa",
                    Region = c.RegionName ?? "Addis Ababa",
                    SubCity = c.SubCityName ?? "N/A",
                    SpecificAddress = c.SpecificAddress ?? identifications.FirstOrDefault(x => x.Description == "SpecificAddress")?.IdNumber ?? "N/A",
                    Phone1 = c.Phone1 ?? "N/A",
                    Email = c.Email ?? identifications.FirstOrDefault(x => x.Description == "ContactInformation")?.IdNumber ?? "N/A",
                    
                    // Infrastructure Mapping
                    StarCategory = identifications.FirstOrDefault(x => x.Description == "StarCategory")?.IdNumber ?? "N/A",
                    TotalRooms = int.TryParse(identifications.FirstOrDefault(x => x.Description == "TotalRooms")?.IdNumber, out var tr) ? tr : 0,
                    TotalBeds = int.TryParse(identifications.FirstOrDefault(x => x.Description == "TotalBeds")?.IdNumber, out var tb) ? tb : 0,
                    // DistanceFromAirport = identifications.FirstOrDefault(x => x.Description == "DistanceFromAirport")?.IdNumber ?? "N/A",
                    ReservationsContact = identifications.FirstOrDefault(x => x.Description == "ReservationsContact")?.IdNumber ?? "N/A",
                    SustainabilityFocalPoint = identifications.FirstOrDefault(x => x.Description == "SustainabilityFocalPoint")?.IdNumber ?? "N/A",

                    // Rooms
                    KingSizeRooms = int.TryParse(identifications.FirstOrDefault(x => x.Description == "KingSizeRooms")?.IdNumber, out var kr) ? kr : 0,
                    TwinBedRooms = int.TryParse(identifications.FirstOrDefault(x => x.Description == "TwinBedRooms")?.IdNumber, out var tbr) ? tbr : 0,
                    JuniorSuites = int.TryParse(identifications.FirstOrDefault(x => x.Description == "JuniorSuites")?.IdNumber, out var js) ? js : 0,
                    Suites = int.TryParse(identifications.FirstOrDefault(x => x.Description == "Suites")?.IdNumber, out var sr) ? sr : 0,
                    PresidentialSuites = int.TryParse(identifications.FirstOrDefault(x => x.Description == "PresidentialSuites")?.IdNumber, out var psr) ? psr : 0,
                    AccessibleRooms = int.TryParse(identifications.FirstOrDefault(x => x.Description == "AccessibleRooms")?.IdNumber, out var ar) ? ar : 0,

                    // F&B
                    AllDayDining = identifications.FirstOrDefault(x => x.Description == "AllDayDining")?.IdNumber?.ToLower() == "true",
                    AllDayDiningSeats = int.TryParse(identifications.FirstOrDefault(x => x.Description == "AllDayDiningSeats")?.IdNumber, out var ads) ? ads : 0,
                    BarsCount = int.TryParse(identifications.FirstOrDefault(x => x.Description == "BarsCount")?.IdNumber, out var bc) ? bc : 0,

                    // Events
                    MeetingRoomsCount = int.TryParse(identifications.FirstOrDefault(x => x.Description == "MeetingRoomsCount")?.IdNumber, out var mrc) ? mrc : 0,
                    LargestRoomCapacityTheatre = int.TryParse(identifications.FirstOrDefault(x => x.Description == "LargestRoomCapacityTheatre")?.IdNumber, out var lrt) ? lrt : 0,

                    // Facilities
                    WifiPropertyWide = identifications.FirstOrDefault(x => x.Description == "WifiPropertyWide")?.IdNumber?.ToLower() == "true",
                    StandbyGeneratorCapacityKva = int.TryParse(identifications.FirstOrDefault(x => x.Description == "StandbyGeneratorCapacityKva")?.IdNumber, out var sgc) ? sgc : 0,
                    
                    ImagePaths = new List<string> { "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?w=800" }
                };

                return PartialView("_EstablishmentDetail", hotel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try 
            {
                // 1. Update Consignee IsActive = true
                var consignees = await _sharedHelpers.GetFilterData<List<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO>>("Consignee", new Dictionary<string, string> { { "id", id.ToString() } });
                if (consignees != null && consignees.Any())
                {
                    var consignee = consignees.First();
                    consignee.IsActive = true;
                    await _sharedHelpers.SendReqAsync<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO, CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO>("Consignee", HttpMethod.Put, consignee);
                }

                // 2. Update Hotel Status
                await _hotelService.UpdateHotelStatusAsync(id, HotelStatus.Approved);
                
                return Json(new { success = true, message = "Establishment approved and activated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string? comment)
        {
            try 
            {
                // 1. Update Consignee IsActive = false
                var consignees = await _sharedHelpers.GetFilterData<List<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO>>("Consignee", new Dictionary<string, string> { { "id", id.ToString() } });
                if (consignees != null && consignees.Any())
                {
                    var consignee = consignees.First();
                    consignee.IsActive = false;
                    await _sharedHelpers.SendReqAsync<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO, CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO>("Consignee", HttpMethod.Put, consignee);
                }

                // 2. Update Hotel Status
                await _hotelService.UpdateHotelStatusAsync(id, HotelStatus.Rejected, comment);
                
                return Json(new { success = true, message = "Establishment declined and deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
