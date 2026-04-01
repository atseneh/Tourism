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
            
            // Limit to top 20
            var Consignees = consignees?.ToList() ?? new List<CNET_V7_Domain.Domain.ViewSchema.VwConsigneeViewDTO>();
            
            return View(Consignees);
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
                        TotalRooms = int.TryParse(identifications.FirstOrDefault(x => x.Description == "TotalRooms")?.IdNumber, out var tr) ? tr : 0,
                        TotalUnits = c.ConsigneeUnitId.HasValue ? 1 : 0, // Simplified: one unit per record in this view
                        TotalSpaces = int.TryParse(identifications.FirstOrDefault(x => x.Description == "MeetingRoomsCount")?.IdNumber, out var ms) ? ms : 0,
                        StarRating = identifications.FirstOrDefault(x => x.Description == "StarCategory")?.IdNumber ?? string.Empty
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
                    .GroupBy(x => x.Category)
                    .Select(g => new RatingSummaryItem {
                        Category = g.Key,
                        PropertyCount = g.Count(),
                        TotalRooms = g.Sum(x => x.TotalRooms),
                        AvgRoomsPerProperty = g.Any() ? Math.Round(g.Average(x => x.TotalRooms), 1) : 0
                    })
                    .OrderByDescending(x => x.Category)
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

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            await _hotelService.UpdateHotelStatusAsync(id, HotelStatus.Approved);
            return RedirectToAction(nameof(Overview));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string comment)
        {
            await _hotelService.UpdateHotelStatusAsync(id, HotelStatus.Rejected, comment);
            return RedirectToAction(nameof(PendingApprovals));
        }
    }
}
