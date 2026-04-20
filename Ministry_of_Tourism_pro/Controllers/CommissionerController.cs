using CNET_V7_Domain.Domain.ConsigneeSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ministry_of_Tourism_pro.Application.DTOs;
using Ministry_of_Tourism_pro.Application.Interfaces;
using Ministry_of_Tourism_pro.Common;
using Ministry_of_Tourism_pro.Domain.Enums;

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
                
                // 1. Fetch establishments
                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(id))
                {
                    parameters.Add("id", id);
                }
                else
                {
                    parameters.Add("gslType", "28"); // Specific test ID from user
                }

                var data = await _sharedHelpers.GetFilterDynamic<List<CNET_V7_Domain.Domain.ViewSchema.VwConsigneeViewDTO>>("VwConsigneeView", parameters);
                
                if (data == null || !data.Any())
                {
                    return Json(report);
                }

                foreach (var c in data)
                {
                    // Fetch Identifications for this establishment
                    var identifications = await _sharedHelpers.GetFilterData<List<IdentificationDTO>>("Identification", new Dictionary<string, string> 
                    { 
                        { "consignee", c.Id.ToString() }
                    }) ?? new List<IdentificationDTO>();

                    var category = c.ChildPreferenceDescrption ?? "General Sector";
                    var tradeName = c.FirstName ?? "Unnamed Establishment";

                    // A. General Registry
                    var generalItem = new GeneralReportItem
                    {
                        Id = c.Id,
                        PropertyName = tradeName,
                        Category = category,
                        City = c.SubCityName ?? "Addis Ababa",
                        Region = c.CityName ?? "Addis Ababa",
                        TIN = c.Tin ?? "N/A",
                        Phone = c.Phone1 ?? "N/A",
                        Email = c.Email ?? identifications.FirstOrDefault(x => x.Description == "ContactInformation")?.IdNumber ?? string.Empty,
                        TotalRooms = int.TryParse(identifications.FirstOrDefault(x => x.Description == "TotalRooms")?.IdNumber, out var tr) ? tr : 0,
                        TotalBeds = int.TryParse(identifications.FirstOrDefault(x => x.Description == "TotalBeds")?.IdNumber, out var tb) ? tb : 0,
                        TotalUnits = c.ConsigneeUnitId.HasValue ? 1 : 0, // Simplified: one unit per record in this view
                        TotalSpaces = int.TryParse(identifications.FirstOrDefault(x => x.Description == "MeetingRoomsCount")?.IdNumber, out var ms) ? ms : 0,
                        StarRating = identifications.FirstOrDefault(x => x.Description == "StarCategory")?.IdNumber ?? string.Empty,
                        ManagerName = identifications.FirstOrDefault(x => x.Description == "ReservationsContact")?.IdNumber ?? string.Empty,
                        SpecificAddress = c.SpecificAddress ?? string.Empty
                    };
                    report.GeneralRegistry.Add(generalItem);

                    // B. Accommodation Infrastructure
                    // Mapping standard room types if they have values > 0
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
                        var countStr = identifications.FirstOrDefault(x => x.Description == rt.Key)?.IdNumber;
                        if (int.TryParse(countStr, out var count) && count > 0)
                        {
                            report.AccommodationInfrastructure.Add(new AccommodationReportItem {
                                PropertyName = tradeName,
                                RoomType = rt.Value,
                                BedConfig = rt.Value.Contains("Twin") ? "Twin" : "Large/King",
                                Count = count,
                                Price = 0, // Price might not be in Identifications type 1
                                MaxPax = 2
                            });
                        }
                    }

                    // C. Food & Beverage
                    var bars = identifications.FirstOrDefault(x => x.Description == "BarsCount")?.IdNumber;
                    if (int.TryParse(bars, out var barCount) && barCount > 0)
                    {
                        report.FoodAndBeverage.Add(new DiningReportItem {
                            PropertyName = tradeName,
                            FacilityName = "Main Bar Area",
                            Specialization = "Beverages",
                            Cuisine = "International",
                            Seating = barCount * 10, // heuristic
                            CateringAvailable = identifications.FirstOrDefault(x => x.Description == "DelegationCatering")?.IdNumber?.ToLower() == "true"
                        });
                    }

                    // D. Meetings & Events
                    var venues = identifications.FirstOrDefault(x => x.Description == "MeetingRoomsCount")?.IdNumber;
                    if (int.TryParse(venues, out var venueCount) && venueCount > 0)
                    {
                        report.MeetingsEvents.Add(new MeetingEventReportItem {
                            PropertyName = tradeName,
                            VenueName = "Conference Hall",
                            Type = "Meeting/Event",
                            SeatingCapacity = int.TryParse(identifications.FirstOrDefault(x => x.Description == "LargestRoomCapacityTheatre")?.IdNumber, out var cap) ? cap : 0,
                            StandingCapacity = 0,
                            AreaSqm = double.TryParse(identifications.FirstOrDefault(x => x.Description == "TotalMeetingSpaceSqm")?.IdNumber, out var area) ? area : 0
                        });
                    }
                }

                // E. Rating Summary
                report.RatingSummary = report.GeneralRegistry
                    .Select(x => {
                        var rating = x.StarRating;
                        if (string.IsNullOrEmpty(rating) || rating == "0") 
                            rating = x.Category.Contains("Star") ? x.Category : "Not Assigned";
                        
                        // Clean up "5 Star" etc.
                        if (!string.IsNullOrEmpty(rating) && char.IsDigit(rating[0]) && !rating.Contains("Star"))
                            rating = rating + " Star";

                        return new { x.TotalRooms, x.TotalBeds, Rating = rating };
                    })
                    .GroupBy(x => x.Rating)
                    .Select(g => new RatingSummaryItem {
                        Category = g.Key,
                        PropertyCount = g.Count(),
                        TotalRooms = g.Sum(x => x.TotalRooms),
                        TotalBeds = g.Sum(x => x.TotalBeds),
                        AvgRoomsPerProperty = g.Any() ? Math.Round(g.Average(x => x.TotalRooms), 1) : 0
                    })
                    .OrderByDescending(x => x.Category == "Not Assigned" ? "0" : x.Category)
                    .ToList();

                return Json(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
