using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ministry_of_Tourism_pro.Application.DTOs;
using Ministry_of_Tourism_pro.Application.Interfaces;
using System.Security.Claims;
using Ministry_of_Tourism_pro.Common;
using Ministry_of_Tourism_pro.WebConstants;

namespace Ministry_of_Tourism_pro.Controllers
{
    [Authorize(Roles = "HotelOwner")]
    public class HotelOwnerController : Controller
    {
        private readonly IHotelService _hotelService;
        private readonly HttpClient _httpClient;
        private readonly SharedHelpers _sharedHelpers;
        
        private static readonly Dictionary<string, int> PropertyToCategoryMapping = new()
        {
            { "RegistrationName", CNET_WebConstantes.CAT_PROPERTY_PROFILE },
            { "SpecificAddress", CNET_WebConstantes.CAT_PROPERTY_PROFILE },
            { "Subcity", CNET_WebConstantes.CAT_PROPERTY_PROFILE },
            { "DistanceFromAirport", CNET_WebConstantes.CAT_PROPERTY_PROFILE },
            { "StarCategory", CNET_WebConstantes.CAT_PROPERTY_PROFILE },
            { "TotalRooms", CNET_WebConstantes.CAT_PROPERTY_PROFILE },
            { "TotalBeds", CNET_WebConstantes.CAT_PROPERTY_PROFILE },
            { "ContactInformation", CNET_WebConstantes.CAT_PROPERTY_PROFILE },
            { "ReservationsContact", CNET_WebConstantes.CAT_PROPERTY_PROFILE },
            { "SustainabilityFocalPoint", CNET_WebConstantes.CAT_PROPERTY_PROFILE },
            { "KingSizeRooms", CNET_WebConstantes.CAT_ROOM_INVENTORY },
            { "TwinBedRooms", CNET_WebConstantes.CAT_ROOM_INVENTORY },
            { "JuniorSuites", CNET_WebConstantes.CAT_ROOM_INVENTORY },
            { "Suites", CNET_WebConstantes.CAT_ROOM_INVENTORY },
            { "PresidentialSuites", CNET_WebConstantes.CAT_ROOM_INVENTORY },
            { "AccessibleRooms", CNET_WebConstantes.CAT_ROOM_INVENTORY },
            { "AllDayDining", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "AllDayDiningSeats", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "SpecialtyRestaurants", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "CoffeeShop", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "BarsCount", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "NightClub", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "SouvenirShops", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "DelegationCatering", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "DelegationCateringMaxPax", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "RefillWaterStations", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "VegVeganOptions", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "NoSingleUsePlastics", CNET_WebConstantes.CAT_FOOD_BEVERAGE_RETAIL },
            { "MeetingRoomsCount", CNET_WebConstantes.CAT_MEETINGS_EVENTS },
            { "LargestRoomCapacityTheatre", CNET_WebConstantes.CAT_MEETINGS_EVENTS },
            { "LargestRoomCapacityClassroom", CNET_WebConstantes.CAT_MEETINGS_EVENTS },
            { "LargestRoomCapacityBanquet", CNET_WebConstantes.CAT_MEETINGS_EVENTS },
            { "TotalMeetingSpaceSqm", CNET_WebConstantes.CAT_MEETINGS_EVENTS },
            { "InternetBandwidthDown", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "InternetBandwidthUp", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "LobbyAreaSqm", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "GreenAreaSqm", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "PoolAvailable", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "PoolType", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "SpaAvailable", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "SpaGender", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "MassageService", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "ChildrensPlayground", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "ChildrenDayCare", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "StaffCanteen", CNET_WebConstantes.CAT_PUBLIC_FACILITIES },
            { "WheelchairRamps", CNET_WebConstantes.CAT_ACCESSIBILITY },
            { "ElevatorsCount", CNET_WebConstantes.CAT_ACCESSIBILITY },
            { "ElevatorsWheelchairSized", CNET_WebConstantes.CAT_ACCESSIBILITY },
            { "PublicAccessibleBathroom", CNET_WebConstantes.CAT_ACCESSIBILITY },
            { "CCTVPublicAreas", CNET_WebConstantes.CAT_SECURITY_SAFETY },
            { "FireExtinguishersLastInspection", CNET_WebConstantes.CAT_SECURITY_SAFETY },
            { "HoseReels", CNET_WebConstantes.CAT_SECURITY_SAFETY },
            { "SmokeDetectorsInRooms", CNET_WebConstantes.CAT_SECURITY_SAFETY },
            { "SmokeDetectorsInPublicAreas", CNET_WebConstantes.CAT_SECURITY_SAFETY },
            { "SprinklerCoverage", CNET_WebConstantes.CAT_SECURITY_SAFETY },
            { "FireAlarmControlPanel", CNET_WebConstantes.CAT_SECURITY_SAFETY },
            { "EmergencyExitsCount", CNET_WebConstantes.CAT_SECURITY_SAFETY },
            { "BagScanner", CNET_WebConstantes.CAT_SECURITY_SAFETY },
            { "WalkThroughScanner", CNET_WebConstantes.CAT_SECURITY_SAFETY },
            { "HandScanner", CNET_WebConstantes.CAT_SECURITY_SAFETY },
            { "ParkingSpacesCount", CNET_WebConstantes.CAT_TRANSPORT_PARKING },
            { "BusParkingCount", CNET_WebConstantes.CAT_TRANSPORT_PARKING },
            { "ShuttleToCop", CNET_WebConstantes.CAT_TRANSPORT_PARKING },
            { "ShuttleToAirport", CNET_WebConstantes.CAT_TRANSPORT_PARKING },
            { "PublicTransportWithin500m", CNET_WebConstantes.CAT_TRANSPORT_PARKING },
            { "EvChargingPoints", CNET_WebConstantes.CAT_TRANSPORT_PARKING },
            { "EvChargerTypes", CNET_WebConstantes.CAT_TRANSPORT_PARKING },
            { "WifiPropertyWide", CNET_WebConstantes.CAT_ICT_GUEST_SERVICES },
            { "WifiAvgSpeed", CNET_WebConstantes.CAT_ICT_GUEST_SERVICES },
            { "InHouseLaundry", CNET_WebConstantes.CAT_ICT_GUEST_SERVICES },
            { "Reception24hr", CNET_WebConstantes.CAT_ICT_GUEST_SERVICES },
            { "VipCheckIn", CNET_WebConstantes.CAT_ICT_GUEST_SERVICES },
            { "PassportScanner", CNET_WebConstantes.CAT_ICT_GUEST_SERVICES },
            { "CurrencyScanner", CNET_WebConstantes.CAT_ICT_GUEST_SERVICES },
            { "StandbyGeneratorCapacityKva", CNET_WebConstantes.CAT_UTILITIES_RESILIENCE },
            { "StandbyGeneratorCoverage", CNET_WebConstantes.CAT_UTILITIES_RESILIENCE },
            { "WaterTreatment", CNET_WebConstantes.CAT_UTILITIES_RESILIENCE },
            { "WasteSegregation", CNET_WebConstantes.CAT_UTILITIES_RESILIENCE },
            { "Recycling", CNET_WebConstantes.CAT_UTILITIES_RESILIENCE },
            { "HazardousWasteHandling", CNET_WebConstantes.CAT_UTILITIES_RESILIENCE },
            { "SustainabilityCertification", CNET_WebConstantes.CAT_SUSTAINABILITY_CERTIFICATIONS },
            { "OtherEcoLabels", CNET_WebConstantes.CAT_SUSTAINABILITY_CERTIFICATIONS },
            { "FoodWasteProgram", CNET_WebConstantes.CAT_SUSTAINABILITY_CERTIFICATIONS },
            { "SustainabilityRefillWaterStations", CNET_WebConstantes.CAT_SUSTAINABILITY_CERTIFICATIONS },
            { "TotalStaff", CNET_WebConstantes.CAT_STAFFING_LANGUAGES },
            { "InternationalLanguagesFrontDesk", CNET_WebConstantes.CAT_STAFFING_LANGUAGES }
        };

        public HotelOwnerController(IHotelService hotelService, IHttpClientFactory httpClientFactory, SharedHelpers sharedHelpers)
        {
            _hotelService = hotelService;
            _httpClient = httpClientFactory.CreateClient("mainclient");
            _sharedHelpers = sharedHelpers;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return RedirectToAction("Login", "Account");

            string? organizationId = null;
            var sessionData = HttpContext.Session.GetString($"GlobalParams_{userName}");
            if (true)
            {
                try
                {
                    var userPerson = await _sharedHelpers.GetUserByUserName(userName);

                    if (userPerson?.Person != null)
                    {
                        var consignee = await _sharedHelpers.GetConsigneeById(userPerson.Person);

                        if (!string.IsNullOrWhiteSpace(consignee?.Tin))
                        {
                            var company = await _sharedHelpers.GetLoggedInCopany(consignee.Tin);

                            if (company?.Id != null)
                            {
                                organizationId = company.Id.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: log exception (ex)
                }
            }

            if (string.IsNullOrEmpty(organizationId)) organizationId = "35"; 

            if (!string.IsNullOrEmpty(organizationId))
            {
                var parameters = new Dictionary<string, string> { { "id", organizationId } };
                var data = await _sharedHelpers.GetFilterDynamic<List<CNET_V7_Domain.Domain.ViewSchema.VwConsigneeViewDTO>>("VwConsigneeView", parameters);
                
                if (data != null && data.Any())
                {
                    var prefParams = new Dictionary<string, string> { { "systemConstant", "28" } };
                    var prefs = await _sharedHelpers.GetFilterData<List<CNET_V7_Domain.Domain.SettingSchema.PreferenceDTO>>("Preference", prefParams);
                    ViewBag.Categories = prefs ?? new List<CNET_V7_Domain.Domain.SettingSchema.PreferenceDTO>();

                    var hotels = new List<HotelDto>();
                    foreach (var c in data)
                    {
                        var h = new HotelDto
                        {
                            Id = c.Id,
                            TradeName = c.FirstName ?? "Unnamed Establishment",
                            RegistrationName = c.SecondName ?? "Unnamed Establishment",
                            TIN = c.Tin,
                            Code = c.Code,
                            Category = c.ChildPreferenceDescrption ?? "General Sector",
                            SpecificAddress = c.SpecificAddress,
                            ConsigneeUnitId = c.ConsigneeUnitId,
                            ConsigneeUnitDescription = c.ConsigneeUnitDescription,
                            AddressLine1 = c.AddressLine1,
                            Status = Ministry_of_Tourism_pro.Domain.Enums.HotelStatus.Approved,
                            City = c.SubCityName ?? "Addis Ababa",
                            Region = c.CityName ?? "Addis Ababa",
                            Email = userName,
                            RejectionComment = c.ChildpreferenceId?.ToString()
                        };

                        // Fetch and map Identification (Infrastructure) fields
                        if (h.ConsigneeUnitId.HasValue)
                        {
                            var identifications = await _sharedHelpers.GetFilterData<List<IdentificationDTO>>("Identification", new Dictionary<string, string> 
                            { 
                                { "consignee", h.Id.ToString() },
                                { "remark", h.ConsigneeUnitId.Value.ToString() }
                                // No type filter to fetch all categories
                            });

                            if (identifications != null)
                            {
                                // Filter by relevant categories if necessary
                                var activeCategories = PropertyToCategoryMapping.Values.Distinct().Concat(new[] { 1 }).ToList();
                                identifications = identifications.Where(x => activeCategories.Contains(x.Type ?? 0)).ToList();
                                
                                MapIdentificationsToHotel(h, identifications);
                            }
                        }
                        hotels.Add(h);
                    }

                    return View(hotels);
                }
            }
            
            var fallback = await _hotelService.GetHotelsByOwnerAsync(userName);
            return View(fallback);
        }

        [HttpPost]
        public async Task<IActionResult> Update(HotelDto model)
        {
            if (ModelState.IsValid)
            {
                await _hotelService.UpdateHotelAsync(model);
                TempData["SuccessMessage"] = "Registry Profile updated.";
                return RedirectToAction(nameof(Dashboard));
            }
            return View("Dashboard", new List<HotelDto> { model });
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateHotelDto model)
        {
            if (ModelState.IsValid)
            {
                //await _hotelService.CreateHotelAsync(model);
                return RedirectToAction(nameof(Dashboard));
            }
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null) return NotFound();

            if (hotel.ConsigneeUnitId.HasValue)
            {
                var identifications = await _sharedHelpers.GetFilterData<List<IdentificationDTO>>("Identification", new Dictionary<string, string> 
                { 
                    { "consignee", hotel.Id.ToString() },
                    { "remark", hotel.ConsigneeUnitId.Value.ToString() }
                });
                
                if (identifications != null)
                {
                    MapIdentificationsToHotel(hotel, identifications);
                }
            }

            return View(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> CommitRegistry(HotelDto model, string mode)
        {
            try
            {
                // The 'model' contains all fields from the registry form.
                // The 'mode' identifies which section is being saved (e.g. 'basic', 'accommodation', or 'all').
                
                switch (mode.ToLower())
                {
                    case "basic":
                        await SyncInfrastructure(model, new[] { "RegistrationName", "SpecificAddress", "Subcity", "DistanceFromAirport", "StarCategory", "TotalRooms", "TotalBeds", "ContactInformation", "ReservationsContact", "SustainabilityFocalPoint" });
                        break;

                    case "accommodation":
                        await SyncInfrastructure(model, new[] { "KingSizeRooms", "TwinBedRooms", "JuniorSuites", "Suites", "PresidentialSuites", "AccessibleRooms" });
                        break;

                    case "food_beverage":
                        await SyncInfrastructure(model, new[] { "AllDayDining", "AllDayDiningSeats", "SpecialtyRestaurants", "CoffeeShop", "BarsCount", "NightClub", "SouvenirShops", "DelegationCatering", "DelegationCateringMaxPax", "RefillWaterStations", "VegVeganOptions", "NoSingleUsePlastics" });
                        break;

                    case "meetings_events":
                        await SyncInfrastructure(model, new[] { "MeetingRoomsCount", "LargestRoomCapacityTheatre", "LargestRoomCapacityClassroom", "LargestRoomCapacityBanquet", "TotalMeetingSpaceSqm" });
                        break;

                    case "public_facilities":
                        await SyncInfrastructure(model, new[] { "InternetBandwidthDown", "InternetBandwidthUp", "LobbyAreaSqm", "GreenAreaSqm", "PoolAvailable", "PoolType", "SpaAvailable", "SpaGender", "MassageService", "ChildrensPlayground", "ChildrenDayCare", "StaffCanteen" });
                        break;

                    case "accessibility":
                        await SyncInfrastructure(model, new[] { "WheelchairRamps", "ElevatorsCount", "ElevatorsWheelchairSized", "PublicAccessibleBathroom" });
                        break;

                    case "security_safety":
                        await SyncInfrastructure(model, new[] { "CCTVPublicAreas", "FireExtinguishersLastInspection", "HoseReels", "SmokeDetectorsInRooms", "SmokeDetectorsInPublicAreas", "SprinklerCoverage", "FireAlarmControlPanel", "EmergencyExitsCount", "BagScanner", "WalkThroughScanner", "HandScanner" });
                        break;

                    case "transport_parking":
                        await SyncInfrastructure(model, new[] { "ParkingSpacesCount", "BusParkingCount", "ShuttleToCop", "ShuttleToAirport", "PublicTransportWithin500m", "EvChargingPoints", "EvChargerTypes" });
                        break;

                    case "ict_services":
                        await SyncInfrastructure(model, new[] { "WifiPropertyWide", "WifiAvgSpeed", "InHouseLaundry", "Reception24hr", "VipCheckIn", "PassportScanner", "CurrencyScanner" });
                        break;

                    case "utilities_resilience":
                        await SyncInfrastructure(model, new[] { "StandbyGeneratorCapacityKva", "StandbyGeneratorCoverage", "WaterTreatment", "WasteSegregation", "Recycling", "HazardousWasteHandling" });
                        break;

                    case "sustainability":
                        await SyncInfrastructure(model, new[] { "SustainabilityCertification", "OtherEcoLabels", "FoodWasteProgram", "SustainabilityRefillWaterStations" });
                        break;

                    case "staffing":
                        await SyncInfrastructure(model, new[] { "TotalStaff", "InternationalLanguagesFrontDesk" });
                        break;

                    case "all":
                        await SyncInfrastructure(model, null); // passing null will sync all non-core fields
                        break;
                }

                // Temporary: Mirror update to the local mock service for UI consistency
                await _hotelService.UpdateHotelAsync(model);

                return Json(new { success = true, message = $"Section '{mode}' received and skeletal logic executed." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetBranchRegistry(int? consigneeUnitId)
        {
            if (!consigneeUnitId.HasValue) return BadRequest("ConsigneeUnitId is required.");

            // 1. Fetch the branch view to get core details
            var parameters = new Dictionary<string, string> { { "consigneeUnitId", consigneeUnitId.Value.ToString() } };
            var data = await _sharedHelpers.GetFilterDynamic<List<VwConsigneeViewDTO>>("VwConsigneeView", parameters);
            var branchView = data?.FirstOrDefault();
            if (branchView == null) return NotFound();

            var branch = new HotelDto
            {
                Id = branchView.Id,
                TradeName = branchView.FirstName ?? "Unnamed Establishment",
                RegistrationName = branchView.SecondName ?? "Unnamed Establishment",
                TIN = branchView.Tin,
                Code = branchView.Code,
                Category = branchView.ChildPreferenceDescrption ?? "General Sector",
                ConsigneeUnitId = branchView.ConsigneeUnitId,
                Subcity = branchView.Subcity?.ToString() ?? ""
            };

            // 2. Fetch and map all identifications for this specific branch
            var identifications = await _sharedHelpers.GetFilterData<List<IdentificationDTO>>("Identification", new Dictionary<string, string> 
            { 
                { "consignee", branch.Id.ToString() },
                { "remark", branch.ConsigneeUnitId.Value.ToString() }
            }) ?? new List<IdentificationDTO>();

            MapIdentificationsToHotel(branch, identifications);

            return Json(branch);
        }

        private async Task<ConsigneeBuffer?> SaveConsigneeBuffer(ConsigneeBuffer buffer)
        {
            try
            {
                HttpResponseMessage response;
                if (buffer.consignee != null && buffer.consignee.Id != 0)
                {
                    response = await _httpClient.PutAsJsonAsync("ConsigneeBuffer", buffer);
                }
                else
                {
                    response = await _httpClient.PostAsJsonAsync("ConsigneeBuffer", buffer);
                }

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ConsigneeBuffer>(responseJson);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SaveConsigneeBuffer: {ex.Message}");
            }
            return null;
        }
        private async Task SyncInfrastructure(HotelDto model, string[]? targetFields)
        {
            // 1. Update Core Identity (Consignee) - mirrors only if it's the main branch or specific requested logic
            var consignees = await _sharedHelpers.GetFilterData<List<ConsigneeDTO>>("Consignee", new Dictionary<string, string> { { "id", model.Id.ToString() } });
            if (consignees != null && consignees.Any())
            {
                var consignee = consignees.First();
                consignee.FirstName = model.TradeName;
                consignee.SecondName = model.RegistrationName;
                await _sharedHelpers.SendReqAsync<ConsigneeDTO, ConsigneeDTO>("Consignee", HttpMethod.Put, consignee);
            }

            // 2. Update Branch/Location (ConsigneeUnit)
            if (model.ConsigneeUnitId.HasValue)
            {
                var units = await _sharedHelpers.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", new Dictionary<string, string> { { "id", model.ConsigneeUnitId.Value.ToString() } });
                if (units != null && units.Any())
                {
                    var unit = units.First();
                    unit.Subcity = int.TryParse(model.Subcity, out int subcityId) ? subcityId : (int?)null;
                    unit.Latitude = model.Latitude;
                    unit.Longitude = model.Longitude;
                    await _sharedHelpers.SendReqAsync<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeUnitDTO, CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeUnitDTO>("ConsigneeUnit", HttpMethod.Put, unit);
                }

                // 3. Persist Infrastructure via IdentificationDTO (Type 1)
                var existingIds = await _sharedHelpers.GetFilterData<List<IdentificationDTO>>("Identification", new Dictionary<string, string> 
                { 
                    { "consignee", model.Id.ToString() },
                    { "remark", model.ConsigneeUnitId.Value.ToString() }
                    // Fetch all categories to check for existing records
                }) ?? new List<IdentificationDTO>();

                var props = typeof(HotelDto).GetProperties();
                var coreFields = new[] { "Id", "ConsigneeUnitId", "TradeName", "Name", "Latitude", "Longitude", "Status", "ImagePaths", "RoomTypes", "Venues", "DiningFacilities" };

                foreach (var prop in props)
                {
                    if (coreFields.Contains(prop.Name)) continue;
                    if (targetFields != null && !targetFields.Contains(prop.Name)) continue;

                    var val = prop.GetValue(model)?.ToString();
                    if (string.IsNullOrEmpty(val)) continue;

                    var existing = existingIds.FirstOrDefault(x => x.Description == prop.Name);
                    if (existing != null)
                    {
                        existing.IdNumber = val;
                        await _sharedHelpers.SendReqAsync<IdentificationDTO, IdentificationDTO>("Identification", HttpMethod.Put, existing);
                    }
                    else
                    {
                        var newId = new IdentificationDTO
                        {
                            Consignee = model.Id,
                            Remark = model.ConsigneeUnitId.Value.ToString(),
                            Type = PropertyToCategoryMapping.TryGetValue(prop.Name, out var catId) ? catId : 1,
                            Description = prop.Name,
                            IdNumber = val,
                        };
                        await _sharedHelpers.SendReqAsync<IdentificationDTO, IdentificationDTO>("Identification", HttpMethod.Post, newId);
                    }
                }
            }
        }

        private void MapIdentificationsToHotel(HotelDto h, List<IdentificationDTO> identifications)
        {
            if (identifications == null) return;
            
            h.RawIdentifications = identifications;
            var hType = typeof(HotelDto);
            foreach (var id in identifications)
            {
                var prop = hType.GetProperty(id.Description ?? "");
                if (prop != null && prop.CanWrite && !string.IsNullOrEmpty(id.IdNumber))
                {
                    try
                    {
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        object? convertedValue = null;

                        if (targetType == typeof(bool))
                        {
                            convertedValue = id.IdNumber.ToLower() == "true";
                        }
                        else if (targetType == typeof(int))
                        {
                            if (int.TryParse(id.IdNumber, out int intVal)) convertedValue = intVal;
                        }
                        else if (targetType == typeof(double))
                        {
                            if (double.TryParse(id.IdNumber, out double dblVal)) convertedValue = dblVal;
                        }
                        else if (targetType == typeof(DateTime))
                        {
                            if (DateTime.TryParse(id.IdNumber, out DateTime dtVal)) convertedValue = dtVal;
                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(id.IdNumber, targetType);
                        }

                        if (convertedValue != null) prop.SetValue(h, convertedValue);
                    }
                    catch { /* Skip mapping errors */ }
                }
            }
        }
    }
}
