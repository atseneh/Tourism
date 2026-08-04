using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Domain.aatmSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ministry_of_Tourism_pro.Application.DTOs;
using Ministry_of_Tourism_pro.Application.Interfaces;
using System.Security.Claims;
using Ministry_of_Tourism_pro.Common;
using Ministry_of_Tourism_pro.WebConstants;
using CNET_V7_Domain.Domain.TransactionSchema;
using Newtonsoft.Json;
using CNET_V7_Domain.Domain.CommonSchema;
using System.Net;
using CNET_V7_Domain.Misc.CommonTypes;

namespace Ministry_of_Tourism_pro.Controllers
{
    [Authorize(Roles = "HotelOwner")]
    public class HotelOwnerController : Controller
    {
        private readonly IHotelService _hotelService;
        private readonly HttpClient _httpClient;
        private readonly SharedHelpers _sharedHelpers;
        private readonly IConfiguration _configuration;

        // FTP settings
        private readonly string FtpFilePath_IP;
        private const string SubDirectory = "GslProfile";
        private const string FtpUserName = "CHM_USER";
        private const string FtpPassword = "AttACHeMenT5&@BBMF@TIIvsDNR";
        private const string FtpBasePath = "/AATM/GslProfile/";

        // Attachment category mapping (index in UI list -> DB system constant ID)
        private static readonly int[] AttachmentCategoryIds = { 1444, 1440, 1448, 1440, 1440, 1451, 1451 };
        private const int ATTACHMENT_TYPE_PICTURE = 1462;
        private const int COMPONENT_CONSIGNEE = 760;

        // Facility type constants for HotelFacilityListDTO
        private const int FACILITY_TYPE_RESTAURANT = 1;
        private const int FACILITY_TYPE_SHOP = 2;
        private const int FACILITY_TYPE_MEETING_ROOM = 3;

        public HotelOwnerController(IHotelService hotelService, IHttpClientFactory httpClientFactory, SharedHelpers sharedHelpers, IConfiguration configuration)
        {
            _hotelService = hotelService;
            _httpClient = httpClientFactory.CreateClient("mainclient");
            _sharedHelpers = sharedHelpers;
            _configuration = configuration;
            FtpFilePath_IP = _configuration["OtherSettings:FtpFilePathIP"] ?? "ftp://196.191.244.132";
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
                            ContactInformation = c.AddressLine1,
                            ReservationsContact = c.AddressLine2,
                            StarCategory = c.NationalId,
                            Status = Ministry_of_Tourism_pro.Domain.Enums.HotelStatus.Approved,
                            City = c.SubCityName ?? "Addis Ababa",
                            Subcity = c.Subcity?.ToString() ?? "",
                            Region = c.CityName ?? "Addis Ababa",
                            Email = userName,
                            RejectionComment = c.ChildpreferenceId?.ToString()
                        };

                        if (h.ConsigneeUnitId.HasValue)
                        {

                            // 2. Fetch infrastructure profile (1-to-1) via new HotelInfrastructureProfileDTO
                            var profiles = await _sharedHelpers.GetFilterData<List<HotelInfrastructureProfileDTO>>("HotelInfrastructureProfile", new Dictionary<string, string> 
                            { 
                                { "consigneeId", h.Id.ToString() },
                                { "consigneeUnitId", h.ConsigneeUnitId.Value.ToString() }
                            });
                            if (profiles != null && profiles.Any())
                            {
                                MapInfrastructureProfileToHotel(h, profiles.First());
                            }

                            // 3. Fetch facility list (1-to-many) via new HotelFacilityListDTO
                            var facilities = await _sharedHelpers.GetFilterData<List<HotelFacilityListDTO>>("HotelFacilityList", new Dictionary<string, string> 
                            { 
                                { "consigneeId", h.Id.ToString() },
                                { "consigneeUnitId", h.ConsigneeUnitId.Value.ToString() }
                            });
                            if (facilities != null && facilities.Any())
                            {
                                MapFacilitiesToHotel(h, facilities);
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

                // Infrastructure profile (1-to-1)
                var profiles = await _sharedHelpers.GetFilterData<List<HotelInfrastructureProfileDTO>>("HotelInfrastructureProfile", new Dictionary<string, string> 
                { 
                    { "consigneeId", hotel.Id.ToString() },
                    { "consigneeUnitId", hotel.ConsigneeUnitId.Value.ToString() }
                });
                if (profiles != null && profiles.Any())
                {
                    MapInfrastructureProfileToHotel(hotel, profiles.First());
                }

                // Facility list (1-to-many)
                var facilities = await _sharedHelpers.GetFilterData<List<HotelFacilityListDTO>>("HotelFacilityList", new Dictionary<string, string> 
                { 
                    { "consigneeId", hotel.Id.ToString() },
                    { "consigneeUnitId", hotel.ConsigneeUnitId.Value.ToString() }
                });
                if (facilities != null && facilities.Any())
                {
                    MapFacilitiesToHotel(hotel, facilities);
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
                        await SyncInfrastructure(model, new[] { "TradeName", "RegistrationName", "SpecificAddress", "Subcity", "StarCategory", "ContactInformation", "ReservationsContact" });
                        break;

                    case "accommodation":
                        await SyncInfrastructure(model, new[] { "KingSizeRooms", "TwinBedRooms", "JuniorSuites", "Suites", "PresidentialSuites", "AccessibleRooms", "VipCheckIn" });
                        break;

                    case "food_beverage":
                        await SyncInfrastructure(model, new[] { "AllDayDining", "AllDayDiningSeats", "SpecialtyRestaurants", "CoffeeShop", "BarsCount", "NightClub", "SouvenirShops", "DelegationCatering", "DelegationCateringMaxPax", "RefillWaterStations", "VegVeganOptions", "NoSingleUsePlastics" });
                        break;

                    case "meetings_events":
                        await SyncInfrastructure(model, new[] { "MeetingRooms" });
                        break;

                    case "public_facilities":
                        await SyncInfrastructure(model, new[] { "InternetBandwidthDown", "InternetBandwidthUp", "LobbyAreaSqm", "GreenAreaSqm", "PoolAvailable", "PoolType", "SpaAvailable", "SpaGender", "MassageService", "ChildrensPlayground", "ChildrenDayCare", "StaffCanteen" });
                        break;

                    case "accessibility":
                        await SyncInfrastructure(model, new[] { "WheelchairRamps", "ElevatorsCount", "ElevatorsWheelchairSized", "PublicAccessibleBathroom" });
                        break;

                    case "safety_security":
                        await SyncInfrastructure(model, new[] { "CCTVPublicAreas", "FireExtinguishersLastInspection", "HoseReels", "SmokeDetectorsInRooms", "SmokeDetectorsInPublicAreas", "SprinklerCoverage", "FireAlarmControlPanel", "EmergencyExitsCount", "BagScanner", "WalkThroughScanner", "HandScanner" });
                        break;

                    case "transport_parking":
                        await SyncInfrastructure(model, new[] { "ParkingSpacesCount", "BusParkingCount", "ValetParking", "ParkingWithin100m", "ShuttleToAirport", "PublicTransportWithin500m", "EvChargingPoints", "EvChargerTypes" });
                        break;

                    case "ict_services":
                        await SyncInfrastructure(model, new[] { "WifiPropertyWide", "WifiAvgSpeed", "InHouseLaundry", "Reception24hr", "PassportScanner", "CurrencyScanner" });
                        break;

                    case "utilities_resilience":
                        await SyncInfrastructure(model, new[] { "StandbyGeneratorCapacityKva", "StandbyGeneratorCoverage", "WaterTreatment", "WasteSegregation", "Recycling", "HazardousWasteHandling" });
                        break;

                    case "sustainability":
                        await SyncInfrastructure(model, new[] { 
                            "SustainabilityFocalPoint", "SustainabilityCertification", "OtherEcoLabels", "FoodWasteProgram", "SustainabilityRefillWaterStations",
                            "TourismServiceCompetenceLicenseCertificate", "FireSafetyCertificate", "EnvironmentalClearanceCertificate", "FoodSafetyAndHygieneCertificate", "IsoCertification"
                        });
                        break;

                    case "staffing":
                        await SyncInfrastructure(model, new[] { "LineStaff", "ManagementStaff", "InternationalLanguagesFrontDesk" });
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
            try 
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
                    Subcity = branchView.Subcity?.ToString() ?? "",
                    ContactInformation = branchView.AddressLine1,
                    ReservationsContact = branchView.AddressLine2,
                    StarCategory = branchView.NationalId
                };


                // 3. Infrastructure profile (1-to-1) via new DTO
                var profiles = await _sharedHelpers.GetFilterData<List<HotelInfrastructureProfileDTO>>("HotelInfrastructureProfile", new Dictionary<string, string> 
                { 
                    { "consigneeId", branch.Id.ToString() },
                    { "consigneeUnitId", branch.ConsigneeUnitId.Value.ToString() }
                });
                if (profiles != null && profiles.Any())
                {
                    MapInfrastructureProfileToHotel(branch, profiles.First());
                }

                // 4. Facility list (1-to-many) via new DTO
                var facilities = await _sharedHelpers.GetFilterData<List<HotelFacilityListDTO>>("HotelFacilityList", new Dictionary<string, string> 
                { 
                    { "consigneeId", branch.Id.ToString() },
                    { "consigneeUnitId", branch.ConsigneeUnitId.Value.ToString() }
                });
                if (facilities != null && facilities.Any())
                {
                    MapFacilitiesToHotel(branch, facilities);
                }

                return Json(branch);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetBranchRegistry Error: {ex.Message}");
                return StatusCode(500, "Error loading registry data: " + ex.Message);
            }
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
            // 1. Update Core Identity (Consignee)
            if (targetFields == null || targetFields.Contains("TradeName") || targetFields.Contains("RegistrationName"))
            {
                var consignees = await _sharedHelpers.GetFilterData<List<ConsigneeDTO>>("Consignee", new Dictionary<string, string> { { "id", model.Id.ToString() } });
                if (consignees != null && consignees.Any())
                {
                    var consignee = consignees.First();
                    if (targetFields == null || targetFields.Contains("TradeName")) consignee.FirstName = model.TradeName;
                    if (targetFields == null || targetFields.Contains("RegistrationName")) consignee.SecondName = model.RegistrationName;
                    if (targetFields == null || targetFields.Contains("StarCategory")) consignee.NationalId = model.StarCategory;
                    await _sharedHelpers.SendReqAsync<ConsigneeDTO, ConsigneeDTO>("Consignee", HttpMethod.Put, consignee);
                }
            }

            // 2. Update Branch/Location (ConsigneeUnit)
            if (model.ConsigneeUnitId.HasValue)
            {
                if (targetFields == null || targetFields.Contains("Subcity") || targetFields.Contains("SpecificAddress") || targetFields.Contains("Latitude") || targetFields.Contains("Longitude"))
                {
                    var units = await _sharedHelpers.GetFilterData<List<ConsigneeUnitDTO>>("ConsigneeUnit", new Dictionary<string, string> { { "id", model.ConsigneeUnitId.Value.ToString() } });
                    if (units != null && units.Any())
                    {
                        var unit = units.First();
                        if (targetFields == null || targetFields.Contains("Subcity"))
                            unit.Subcity = int.TryParse(model.Subcity, out int subcityId) ? subcityId : (int?)null;
                        if (targetFields == null || targetFields.Contains("SpecificAddress"))
                            unit.SpecificAddress = model.SpecificAddress;
                        if (targetFields == null || targetFields.Contains("ContactInformation"))
                            unit.AddressLine1 = model.ContactInformation;
                        if (targetFields == null || targetFields.Contains("ReservationsContact"))
                            unit.AddressLine2 = model.ReservationsContact;
                        if (targetFields == null || targetFields.Contains("Latitude"))
                            unit.Latitude = model.Latitude;
                        if (targetFields == null || targetFields.Contains("Longitude"))
                            unit.Longitude = model.Longitude;
                            
                        await _sharedHelpers.SendReqAsync<ConsigneeUnitDTO, ConsigneeUnitDTO>("ConsigneeUnit", HttpMethod.Put, unit);
                    }
                }

                // 3. Infrastructure profile via HotelInfrastructureProfileDTO (1-to-1)
                await SyncInfrastructureProfile(model, targetFields);

                // 5. Facility list via HotelFacilityListDTO (1-to-many)
                await SyncFacilityList(model, targetFields);
            }
        }


        /// <summary>Saves all infrastructure fields via HotelInfrastructureProfileDTO (1-to-1 upsert).</summary>
        private async Task SyncInfrastructureProfile(HotelDto model, string[]? targetFields)
        {
            var existing = await _sharedHelpers.GetFilterData<List<HotelInfrastructureProfileDTO>>("HotelInfrastructureProfile", new Dictionary<string, string>
            {
                { "consigneeId", model.Id.ToString() },
                { "consigneeUnitId", model.ConsigneeUnitId!.Value.ToString() }
            });

            var profile = existing?.FirstOrDefault() ?? new HotelInfrastructureProfileDTO
            {
                ConsigneeId = model.Id,
                ConsigneeUnitId = model.ConsigneeUnitId
            };

            bool changed = false;

            void SetIfTarget<T>(string fieldName, T value, Action<T> setter)
            {
                if (targetFields == null || targetFields.Contains(fieldName))
                {
                    setter(value);
                    changed = true;
                }
            }

            // Map all infrastructure fields from HotelDto → profile
            SetIfTarget("VipCheckIn", model.VipCheckIn, v => profile.VipCheckIn = v);
            SetIfTarget("KingSizeRooms", model.KingSizeRooms, v => profile.KingSizeRooms = v);
            SetIfTarget("TwinBedRooms", model.TwinBedRooms, v => profile.TwinBedRooms = v);
            SetIfTarget("JuniorSuites", model.JuniorSuites, v => profile.JuniorSuites = v);
            SetIfTarget("Suites", model.Suites, v => profile.Suites = v);
            SetIfTarget("PresidentialSuites", model.PresidentialSuites, v => profile.PresidentialSuites = v);
            SetIfTarget("AccessibleRooms", model.AccessibleRooms, v => profile.AccessibleRooms = v);
            SetIfTarget("InHouseLaundry", model.InHouseLaundry, v => profile.InHouseLaundry = v);
            SetIfTarget("Reception24hr", model.Reception24hr, v => profile.Reception24hr = v);
            SetIfTarget("AllDayDining", model.AllDayDining, v => profile.AllDayDining = v);
            SetIfTarget("AllDayDiningSeats", model.AllDayDiningSeats, v => profile.AllDayDiningSeats = v);
            SetIfTarget("CoffeeShop", model.CoffeeShop, v => profile.CoffeeShop = v);
            SetIfTarget("BarsCount", model.BarsCount, v => profile.BarsCount = v);
            SetIfTarget("NightClub", model.NightClub, v => profile.NightClub = v);
            SetIfTarget("DelegationCatering", model.DelegationCatering, v => profile.DelegationCatering = v);
            SetIfTarget("DelegationCateringMaxPax", model.DelegationCateringMaxPax, v => profile.DelegationCateringMaxPax = v);
            SetIfTarget("RefillWaterStations", model.RefillWaterStations, v => profile.RefillWaterStations = v);
            SetIfTarget("VegVeganOptions", model.VegVeganOptions, v => profile.VegVeganOptions = v);
            SetIfTarget("NoSingleUsePlastics", model.NoSingleUsePlastics, v => profile.NoSingleUsePlastics = v);
            SetIfTarget("LobbyAreaSqm", (decimal?)model.LobbyAreaSqm, v => profile.LobbyAreaSqm = v);
            SetIfTarget("GreenAreaSqm", (decimal?)model.GreenAreaSqm, v => profile.GreenAreaSqm = v);
            SetIfTarget("PoolAvailable", model.PoolAvailable, v => profile.PoolAvailable = v);
            SetIfTarget("PoolType", model.PoolType, v => profile.PoolType = v);
            SetIfTarget("SpaAvailable", model.SpaAvailable, v => profile.SpaAvailable = v);
            SetIfTarget("SpaGender", model.SpaGender, v => profile.SpaGender = v);
            SetIfTarget("MassageService", model.MassageService, v => profile.MassageService = v);
            SetIfTarget("ChildrensPlayground", model.ChildrensPlayground, v => profile.ChildrensPlayground = v);
            SetIfTarget("ChildrenDayCare", model.ChildrenDayCare, v => profile.ChildrenDayCare = v);
            SetIfTarget("StaffCanteen", model.StaffCanteen, v => profile.StaffCanteen = v);
            SetIfTarget("WheelchairRamps", model.WheelchairRamps, v => profile.WheelchairRamps = v);
            SetIfTarget("ElevatorsCount", model.ElevatorsCount, v => profile.ElevatorsCount = v);
            SetIfTarget("ElevatorsWheelchairSized", model.ElevatorsWheelchairSized, v => profile.ElevatorsWheelchairSized = v);
            SetIfTarget("PublicAccessibleBathroom", model.PublicAccessibleBathroom, v => profile.PublicAccessibleBathroom = v);
            SetIfTarget("CCTVPublicAreas", model.CCTVPublicAreas, v => profile.CctvpublicAreas = v);
            SetIfTarget("FireExtinguishersLastInspection", model.FireExtinguishersLastInspection, v => profile.FireExtinguishersLastInspection = v);
            SetIfTarget("HoseReels", model.HoseReels, v => profile.HoseReels = v);
            SetIfTarget("SmokeDetectorsInRooms", model.SmokeDetectorsInRooms, v => profile.SmokeDetectorsInRooms = v);
            SetIfTarget("SmokeDetectorsInPublicAreas", model.SmokeDetectorsInPublicAreas, v => profile.SmokeDetectorsInPublicAreas = v);
            SetIfTarget("SprinklerCoverage", model.SprinklerCoverage, v => profile.SprinklerCoverage = v);
            SetIfTarget("FireAlarmControlPanel", model.FireAlarmControlPanel, v => profile.FireAlarmControlPanel = v);
            SetIfTarget("EmergencyExitsCount", model.EmergencyExitsCount, v => profile.EmergencyExitsCount = v);
            SetIfTarget("BagScanner", model.BagScanner, v => profile.BagScanner = v);
            SetIfTarget("WalkThroughScanner", model.WalkThroughScanner, v => profile.WalkThroughScanner = v);
            SetIfTarget("HandScanner", model.HandScanner, v => profile.HandScanner = v);
            SetIfTarget("ElectronicDoorlock", model.ElectronicDoorlock, v => profile.ElectronicDoorlock = v);
            SetIfTarget("ElevatorFloorController", model.ElevatorFloorController, v => profile.ElevatorFloorController = v);
            SetIfTarget("OncallDoctor", model.OncallDoctor, v => profile.OncallDoctor = v);
            SetIfTarget("ParkingSpacesCount", model.ParkingSpacesCount, v => profile.ParkingSpacesCount = v);
            SetIfTarget("BusParkingCount", model.BusParkingCount, v => profile.BusParkingCount = v);
            SetIfTarget("ValetParking", model.ValetParking, v => profile.ValetParking = v);
            SetIfTarget("ParkingWithin100m", model.ParkingWithin100m, v => profile.ParkingWithin100m = v);
            SetIfTarget("ShuttleToAirport", model.ShuttleToAirport, v => profile.ShuttleToAirport = v);
            SetIfTarget("PublicTransportWithin500m", model.PublicTransportWithin500m, v => profile.PublicTransportWithin500m = v);
            SetIfTarget("EvChargingPoints", model.EvChargingPoints, v => profile.EvChargingPoints = v);
            SetIfTarget("EvChargerTypes", model.EvChargerTypes, v => profile.EvChargerTypes = v);
            SetIfTarget("WifiPropertyWide", model.WifiPropertyWide, v => profile.WifiPropertyWide = v);
            SetIfTarget("WifiAvgSpeed", (decimal?)model.WifiAvgSpeed, v => profile.WifiAvgSpeed = v);
            SetIfTarget("InternetBandwidthDown", (decimal?)model.InternetBandwidthDown, v => profile.InternetBandwidthDown = v);
            SetIfTarget("InternetBandwidthUp", (decimal?)model.InternetBandwidthUp, v => profile.InternetBandwidthUp = v);
            SetIfTarget("PassportScanner", model.PassportScanner, v => profile.PassportScanner = v);
            SetIfTarget("CurrencyScanner", model.CurrencyScanner, v => profile.CurrencyScanner = v);
            SetIfTarget("OnlineOrderingSystem", model.OnlineOrderingSystem, v => profile.OnlineOrderingSystem = v);
            SetIfTarget("OnlineBookingSystem", model.OnlineBookingSystem, v => profile.OnlineBookingSystem = v);
            SetIfTarget("TableReservation", model.TableReservation, v => profile.TableReservation = v);
            SetIfTarget("IpTv", model.IpTv, v => profile.IpTv = v);
            SetIfTarget("StandbyGeneratorCapacityKva", (decimal?)model.StandbyGeneratorCapacityKva, v => profile.StandbyGeneratorCapacityKva = v);
            SetIfTarget("StandbyGeneratorCoverage", model.StandbyGeneratorCoverage, v => profile.StandbyGeneratorCoverage = v);
            SetIfTarget("WaterTreatment", model.WaterTreatment, v => profile.WaterTreatment = v);
            SetIfTarget("WasteSegregation", model.WasteSegregation, v => profile.WasteSegregation = v);
            SetIfTarget("Recycling", model.Recycling, v => profile.Recycling = v);
            SetIfTarget("HazardousWasteHandling", model.HazardousWasteHandling, v => profile.HazardousWasteHandling = v);
            SetIfTarget("SustainabilityFocalPoint", model.SustainabilityFocalPoint, v => profile.SustainabilityFocalPoint = v);
            SetIfTarget("SustainabilityCertification", model.SustainabilityCertification, v => profile.SustainabilityCertification = v);
            SetIfTarget("OtherEcoLabels", model.OtherEcoLabels, v => profile.OtherEcoLabels = v);
            SetIfTarget("FoodWasteProgram", model.FoodWasteProgram, v => profile.FoodWasteProgram = v);
            SetIfTarget("SustainabilityRefillWaterStations", model.SustainabilityRefillWaterStations, v => profile.SustainabilityRefillWaterStations = v);
            SetIfTarget("TourismServiceCompetenceLicenseCertificate", model.TourismServiceCompetenceLicenseCertificate, v => profile.TourismServiceCompetenceLicenseCertificate = v);
            SetIfTarget("FireSafetyCertificate", model.FireSafetyCertificate, v => profile.FireSafetyCertificate = v);
            SetIfTarget("EnvironmentalClearanceCertificate", model.EnvironmentalClearanceCertificate, v => profile.EnvironmentalClearanceCertificate = v);
            SetIfTarget("FoodSafetyAndHygieneCertificate", model.FoodSafetyAndHygieneCertificate, v => profile.FoodSafetyAndHygieneCertificate = v);
            SetIfTarget("IsoCertification", model.IsoCertification, v => profile.IsoCertification = v);
            SetIfTarget("LineStaff", model.LineStaff, v => profile.LineStaff = v);
            SetIfTarget("ManagementStaff", model.ManagementStaff, v => profile.ManagementStaff = v);
            SetIfTarget("InternationalLanguagesFrontDesk", model.InternationalLanguagesFrontDesk, v => profile.InternationalLanguagesFrontDesk = v);

            if (changed || profile.Id == 0)
            {
                var method = profile.Id != 0 ? HttpMethod.Put : HttpMethod.Post;
                await _sharedHelpers.SendReqAsync<HotelInfrastructureProfileDTO, HotelInfrastructureProfileDTO>("HotelInfrastructureProfile", method, profile);
            }
        }

        /// <summary>Saves facility list items via HotelFacilityListDTO (delete+re-insert).</summary>
        private async Task SyncFacilityList(HotelDto model, string[]? targetFields)
        {
            var facilityJsonFields = new Dictionary<string, int>
            {
                { "SpecialtyRestaurants", FACILITY_TYPE_RESTAURANT },
                { "SouvenirShops", FACILITY_TYPE_SHOP },
                { "MeetingRooms", FACILITY_TYPE_MEETING_ROOM }
            };

            foreach (var entry in facilityJsonFields)
            {
                if (targetFields != null && !targetFields.Contains(entry.Key)) continue;

                var prop = typeof(HotelDto).GetProperty(entry.Key);
                var val = prop?.GetValue(model)?.ToString();
                if (string.IsNullOrEmpty(val)) continue;

                try
                {
                    var list = JsonConvert.DeserializeObject<List<dynamic>>(val ?? "[]");
                    if (list == null) continue;

                    // Delete existing facilities of this type
                    var existingFacilities = await _sharedHelpers.GetFilterData<List<HotelFacilityListDTO>>("HotelFacilityList", new Dictionary<string, string>
                    {
                        { "consigneeId", model.Id.ToString() },
                        { "consigneeUnitId", model.ConsigneeUnitId!.Value.ToString() },
                        { "facilityTypeConstantId", entry.Value.ToString() }
                    });
                    if (existingFacilities != null)
                    {
                        foreach (var ef in existingFacilities)
                            await _sharedHelpers.SendReqAsync<object, object>($"HotelFacilityList/{ef.Id}", HttpMethod.Delete);
                    }

                    // Insert new facilities
                    foreach (var item in list)
                    {
                        var facility = new HotelFacilityListDTO
                        {
                            ConsigneeId = model.Id,
                            ConsigneeUnitId = model.ConsigneeUnitId,
                            FacilityTypeConstantId = entry.Value
                        };

                        if (entry.Value == FACILITY_TYPE_RESTAURANT)
                        {
                            facility.Name = (string?)item.name ?? "";
                            facility.Cuisine = (string?)item.cuisine;
                            facility.Capacity = (int?)item.capacity;
                            facility.IsHalal = item.halal == true;
                            facility.IsVegan = item.vegan == true;
                            facility.Notes = (string?)item.notes;
                            facility.Remark = (string?)item.type;
                        }
                        else if (entry.Value == FACILITY_TYPE_SHOP)
                        {
                            facility.Name = (string?)item.name ?? "";
                            facility.Location = (string?)item.location;
                            facility.OperatingHours = (string?)item.hours;
                            facility.Notes = (string?)item.notes;
                            facility.Remark = (string?)item.category;
                        }
                        else if (entry.Value == FACILITY_TYPE_MEETING_ROOM)
                        {
                            facility.Name = (string?)item.name ?? "";
                            facility.Width = (decimal?)item.width;
                            facility.Length = (decimal?)item.length;
                            facility.CeilingHeight = (decimal?)item.ceilingHeight;
                            facility.Capacity = (int?)item.capacity;
                            facility.SettingArrangement = (string?)item.setting;
                            facility.Remark = (string?)item.type;
                        }

                        await _sharedHelpers.SendReqAsync<HotelFacilityListDTO, HotelFacilityListDTO>("HotelFacilityList", HttpMethod.Post, facility);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error syncing {entry.Key} to HotelFacilityList: {ex.Message}");
                }
            }
        }
       

        /// <summary>Maps HotelInfrastructureProfileDTO → flat HotelDto fields (1-to-1 read).</summary>
        private void MapInfrastructureProfileToHotel(HotelDto h, HotelInfrastructureProfileDTO p)
        {
            h.InfrastructureProfile = p;
            h.VipCheckIn = p.VipCheckIn;
            h.KingSizeRooms = p.KingSizeRooms;
            h.TwinBedRooms = p.TwinBedRooms;
            h.JuniorSuites = p.JuniorSuites;
            h.Suites = p.Suites;
            h.PresidentialSuites = p.PresidentialSuites;
            h.AccessibleRooms = p.AccessibleRooms;
            h.InHouseLaundry = p.InHouseLaundry;
            h.Reception24hr = p.Reception24hr;
            h.AllDayDining = p.AllDayDining;
            h.AllDayDiningSeats = p.AllDayDiningSeats;
            h.CoffeeShop = p.CoffeeShop;
            h.BarsCount = p.BarsCount;
            h.NightClub = p.NightClub;
            h.DelegationCatering = p.DelegationCatering;
            h.DelegationCateringMaxPax = p.DelegationCateringMaxPax;
            h.RefillWaterStations = p.RefillWaterStations;
            h.VegVeganOptions = p.VegVeganOptions;
            h.NoSingleUsePlastics = p.NoSingleUsePlastics;
            h.LobbyAreaSqm = (double?)p.LobbyAreaSqm;
            h.GreenAreaSqm = (double?)p.GreenAreaSqm;
            h.PoolAvailable = p.PoolAvailable;
            h.PoolType = p.PoolType;
            h.SpaAvailable = p.SpaAvailable;
            h.SpaGender = p.SpaGender;
            h.MassageService = p.MassageService;
            h.ChildrensPlayground = p.ChildrensPlayground;
            h.ChildrenDayCare = p.ChildrenDayCare;
            h.StaffCanteen = p.StaffCanteen;
            h.WheelchairRamps = p.WheelchairRamps;
            h.ElevatorsCount = p.ElevatorsCount;
            h.ElevatorsWheelchairSized = p.ElevatorsWheelchairSized;
            h.PublicAccessibleBathroom = p.PublicAccessibleBathroom;
            h.CCTVPublicAreas = p.CctvpublicAreas;
            h.FireExtinguishersLastInspection = p.FireExtinguishersLastInspection;
            h.HoseReels = p.HoseReels;
            h.SmokeDetectorsInRooms = p.SmokeDetectorsInRooms;
            h.SmokeDetectorsInPublicAreas = p.SmokeDetectorsInPublicAreas;
            h.SprinklerCoverage = p.SprinklerCoverage;
            h.FireAlarmControlPanel = p.FireAlarmControlPanel;
            h.EmergencyExitsCount = p.EmergencyExitsCount;
            h.BagScanner = p.BagScanner;
            h.WalkThroughScanner = p.WalkThroughScanner;
            h.HandScanner = p.HandScanner;
            h.ElectronicDoorlock = p.ElectronicDoorlock;
            h.ElevatorFloorController = p.ElevatorFloorController;
            h.OncallDoctor = p.OncallDoctor;
            h.ParkingSpacesCount = p.ParkingSpacesCount;
            h.BusParkingCount = p.BusParkingCount;
            h.ValetParking = p.ValetParking;
            h.ParkingWithin100m = p.ParkingWithin100m;
            h.ShuttleToAirport = p.ShuttleToAirport;
            h.PublicTransportWithin500m = p.PublicTransportWithin500m;
            h.EvChargingPoints = p.EvChargingPoints;
            h.EvChargerTypes = p.EvChargerTypes;
            h.WifiPropertyWide = p.WifiPropertyWide;
            h.WifiAvgSpeed = (double?)p.WifiAvgSpeed;
            h.InternetBandwidthDown = (double?)p.InternetBandwidthDown;
            h.InternetBandwidthUp = (double?)p.InternetBandwidthUp;
            h.PassportScanner = p.PassportScanner;
            h.CurrencyScanner = p.CurrencyScanner;
            h.OnlineOrderingSystem = p.OnlineOrderingSystem;
            h.OnlineBookingSystem = p.OnlineBookingSystem;
            h.TableReservation = p.TableReservation;
            h.IpTv = p.IpTv;
            h.StandbyGeneratorCapacityKva = (double?)p.StandbyGeneratorCapacityKva;
            h.StandbyGeneratorCoverage = p.StandbyGeneratorCoverage;
            h.WaterTreatment = p.WaterTreatment;
            h.WasteSegregation = p.WasteSegregation;
            h.Recycling = p.Recycling;
            h.HazardousWasteHandling = p.HazardousWasteHandling;
            h.SustainabilityFocalPoint = p.SustainabilityFocalPoint;
            h.SustainabilityCertification = p.SustainabilityCertification;
            h.OtherEcoLabels = p.OtherEcoLabels;
            h.FoodWasteProgram = p.FoodWasteProgram;
            h.SustainabilityRefillWaterStations = p.SustainabilityRefillWaterStations;
            h.TourismServiceCompetenceLicenseCertificate = p.TourismServiceCompetenceLicenseCertificate;
            h.FireSafetyCertificate = p.FireSafetyCertificate;
            h.EnvironmentalClearanceCertificate = p.EnvironmentalClearanceCertificate;
            h.FoodSafetyAndHygieneCertificate = p.FoodSafetyAndHygieneCertificate;
            h.IsoCertification = p.IsoCertification;
            h.LineStaff = p.LineStaff;
            h.ManagementStaff = p.ManagementStaff;
            h.TotalStaff = (p.LineStaff ?? 0) + (p.ManagementStaff ?? 0);
            h.InternationalLanguagesFrontDesk = p.InternationalLanguagesFrontDesk;

            // Calculate aggregate accommodation fields
            h.TotalRooms = (p.KingSizeRooms ?? 0) + (p.TwinBedRooms ?? 0) + (p.JuniorSuites ?? 0) + 
                           (p.Suites ?? 0) + (p.PresidentialSuites ?? 0) + (p.AccessibleRooms ?? 0);
            
            h.TotalBeds = (p.KingSizeRooms ?? 0) + 
                          ((p.TwinBedRooms ?? 0) * 2) + 
                          ((p.JuniorSuites ?? 0) * 1) + 
                          ((p.Suites ?? 0) * 3) + 
                          ((p.PresidentialSuites ?? 0) * 3) + 
                          (p.AccessibleRooms ?? 0);
        }

        /// <summary>Maps List of HotelFacilityListDTO → HotelDto JSON fields (1-to-many read).</summary>
        private void MapFacilitiesToHotel(HotelDto h, List<HotelFacilityListDTO> facilities)
        {
            h.Facilities = facilities;
            var restaurants = new List<object>();
            var shops = new List<object>();
            var meetings = new List<object>();

            foreach (var f in facilities)
            {
                if (f.FacilityTypeConstantId == FACILITY_TYPE_RESTAURANT)
                {
                    restaurants.Add(new
                    {
                        id = f.Id, name = f.Name, type = f.Remark,
                        cuisine = f.Cuisine, capacity = f.Capacity,
                        halal = f.IsHalal, vegan = f.IsVegan, notes = f.Notes
                    });
                }
                else if (f.FacilityTypeConstantId == FACILITY_TYPE_SHOP)
                {
                    shops.Add(new
                    {
                        id = f.Id, name = f.Name, type = f.Remark,
                        location = f.Location, hours = f.OperatingHours, notes = f.Notes
                    });
                }
                else if (f.FacilityTypeConstantId == FACILITY_TYPE_MEETING_ROOM)
                {
                    meetings.Add(new
                    {
                        id = f.Id, name = f.Name, type = f.Remark,
                        width = (double?)f.Width, length = (double?)f.Length, ceilingHeight = (double?)f.CeilingHeight,
                        capacity = f.Capacity, setting = f.SettingArrangement
                    });
                }
            }
            h.SpecialtyRestaurants = Newtonsoft.Json.JsonConvert.SerializeObject(restaurants);
            h.SouvenirShops = Newtonsoft.Json.JsonConvert.SerializeObject(shops);
            h.MeetingRooms = Newtonsoft.Json.JsonConvert.SerializeObject(meetings);

            // Calculate aggregates for meetings and events (not saved in DB)
            if (meetings.Any())
            {
                h.MeetingRoomsCount = meetings.Count;
                double totalSqm = 0;
                int maxTheater = 0, maxClassroom = 0, maxBanquet = 0;

                foreach (dynamic m in meetings)
                {
                    double w = m.width ?? 0;
                    double l = m.length ?? 0;
                    totalSqm += (w * l);

                    int cap = m.capacity ?? 0;
                    string type = m.type ?? "";
                    if (type != null && type.Contains("Theater") && cap > maxTheater) maxTheater = cap;
                    if (type != null && type.Contains("Classroom") && cap > maxClassroom) maxClassroom = cap;
                    if (type != null && type.Contains("Banquet") && cap > maxBanquet) maxBanquet = cap;
                }

                h.TotalMeetingSpaceSqm = totalSqm;
                h.LargestRoomCapacityTheatre = maxTheater;
                h.LargestRoomCapacityClassroom = maxClassroom;
                h.LargestRoomCapacityBanquet = maxBanquet;
            }
            else
            {
                h.MeetingRoomsCount = 0;
                h.TotalMeetingSpaceSqm = 0;
                h.LargestRoomCapacityTheatre = 0;
                h.LargestRoomCapacityClassroom = 0;
                h.LargestRoomCapacityBanquet = 0;
            }
        }

        #region Attachment Methods

        [HttpPost]
        public async Task<IActionResult> UploadAttachment(IFormFile file, int consigneeId, int categoryIndex)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "No file selected." });

                // Generate safe unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                uniqueFileName = uniqueFileName.Replace(" ", "").Replace("+", "").Replace("%", "")
                    .Replace(",", "").Replace("&", "").Replace("*", "").Replace("@", "");

                string ftpFullFilePath = FtpBasePath + uniqueFileName;

                // Upload to FTP
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(FtpFilePath_IP + ftpFullFilePath);
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
                ftpRequest.ContentLength = fileBytes.Length;

                using (Stream requestStream = ftpRequest.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                }

                // Get the DB category ID from the UI index
                int dbCategory = (categoryIndex >= 0 && categoryIndex < AttachmentCategoryIds.Length)
                    ? AttachmentCategoryIds[categoryIndex]
                    : 1451; // default to Reference Documents

                // Save attachment metadata via API
                var attachment = new AttachmentDTO
                {
                    Reference = consigneeId,
                    Category = dbCategory,
                    Description = file.FileName,
                    Type = ATTACHMENT_TYPE_PICTURE,
                    Url = ftpFullFilePath,
                    Pointer = COMPONENT_CONSIGNEE,
                    Index = categoryIndex,
                    CreatedOn = DateTime.Now,
                    LastModified = DateTime.Now
                };

                var response = await _httpClient.PostAsJsonAsync("Attachment", attachment);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Attachment saved successfully!" });
                }
                else
                {
                    var errContent = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = "API save failed: " + errContent });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Upload error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAttachments(int consigneeId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Attachment/filter?reference={consigneeId}&pointer={COMPONENT_CONSIGNEE}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var attachments = JsonConvert.DeserializeObject<List<AttachmentDTO>>(content);

                    var result = attachments?.Select(a => new
                    {
                        id = a.Id,
                        description = a.Description,
                        category = a.Category,
                        categoryIndex = a.Index,
                        url = "/HotelOwner/GetAttachmentFile?url=" + Uri.EscapeDataString(a.Url ?? ""),
                        createdOn = a.CreatedOn
                    }).ToList();

                    return Json(new { success = true, data = result });
                }
                return Json(new { success = false, data = new List<object>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = new List<object>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAttachmentFile(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return BadRequest("File URL is empty.");

                string ftpFullPath = FtpFilePath_IP + url;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFullPath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(FtpUserName, FtpPassword);

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                using (Stream responseStream = response.GetResponseStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    await responseStream.CopyToAsync(ms);
                    byte[] fileBytes = ms.ToArray();

                    // Determine MIME type
                    string contentType = "application/octet-stream";
                    string ext = Path.GetExtension(url).ToLower();
                    if (ext == ".pdf") contentType = "application/pdf";
                    else if (ext == ".jpg" || ext == ".jpeg") contentType = "image/jpeg";
                    else if (ext == ".png") contentType = "image/png";
                    else if (ext == ".gif") contentType = "image/gif";
                    else if (ext == ".webp") contentType = "image/webp";

                    return File(fileBytes, contentType);
                }
            }
            catch (Exception ex)
            {
                return NotFound("Error loading attachment file: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAttachment(int id)
        {
            try
            {
                // Get the attachment record first to find the FTP path
                var getResponse = await _httpClient.GetAsync($"Attachment/filter?id={id}");
                AttachmentDTO? attachmentRecord = null;
                if (getResponse.IsSuccessStatusCode)
                {
                    var getContent = await getResponse.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<AttachmentDTO>>(getContent);
                    attachmentRecord = list?.FirstOrDefault();
                }

                // Delete from API
                var deleteResponse = await _httpClient.DeleteAsync("Attachment/" + id);
                if (!deleteResponse.IsSuccessStatusCode)
                    return Json(new { success = false, message = "Failed to delete from database." });

                // Delete from FTP
                if (attachmentRecord != null && !string.IsNullOrEmpty(attachmentRecord.Url))
                {
                    try
                    {
                        string ftpDeletePath = FtpFilePath_IP + attachmentRecord.Url;
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpDeletePath);
                        request.Method = WebRequestMethods.Ftp.DeleteFile;
                        request.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
                        FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
                        ftpResponse.Close();
                    }
                    catch { /* FTP delete is best-effort */ }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion


        #region Change Password / Profile

        [HttpPost]
        public async Task<IActionResult> changepassworddetail([FromBody] Ministry_of_Tourism_pro.Models.SecurityModel changepass)
        {
            if (changepass == null)
                return Json(new { result = "Invalid request" });

            var currentUser = User.Identity?.Name ?? string.Empty;
            if (string.IsNullOrEmpty(currentUser))
                return Json(new { result = "Unauthorized" });

            changepass.cha_username = currentUser;

            if (string.IsNullOrWhiteSpace(changepass.cha_oldpasword))
                return Json(new { result = "Please enter your current password" });

            bool isUsernameChange = !string.IsNullOrWhiteSpace(changepass.cha_newusername) &&
                                    !string.Equals(currentUser, changepass.cha_newusername.Trim(), StringComparison.OrdinalIgnoreCase);

            bool isPasswordChange = !string.IsNullOrWhiteSpace(changepass.cha_newpassword) &&
                                    !string.Equals(changepass.cha_oldpasword, changepass.cha_newpassword);

            if (!isUsernameChange && !isPasswordChange)
                return Json(new { result = "No changes requested" });

            if (isPasswordChange)
            {
                if (changepass.cha_newpassword != changepass.cha_confirmpassord)
                    return Json(new { result = "New passwords do not match" });

                if (changepass.cha_newpassword.Length < 6)
                    return Json(new { result = "Password must be at least 6 characters" });
            }

            var authManager = new Ministry_of_Tourism_pro.Common.AuthenticationManager(
                HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>(),
                HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>(),
                _sharedHelpers);

            var authResult = await authManager.AuthenticateUser(currentUser, changepass.cha_oldpasword, CNET_WebConstantes.HARDCODED_BRANCH.ToString());
            if (authResult == null || !authResult.Success || authResult.Data == null)
                return Json(new { result = "Old Password is incorrect" });

            var muser = await _sharedHelpers.GetUserByUserName(currentUser);
            if (muser == null)
                return Json(new { result = "User not found" });

            string targetUsername = isUsernameChange ? changepass.cha_newusername.Trim() : muser.UserName;

            if (isUsernameChange)
            {
                var existingUser = await _sharedHelpers.GetUserByUserName(targetUsername);
                if (existingUser != null && existingUser.Id != muser.Id)
                    return Json(new { result = "Username already exists. Please choose a different username." });
            }

            var reuser = new UserUpdateDTO
            {
                userId         = Convert.ToInt32(muser.Id),
                oldUserName    = muser.UserName,
                newUserName    = targetUsername,
                person         = muser.Person,
                isActive       = changepass.cha_Isactive,
                isAdmin        = true,
                changePassword = isPasswordChange,
                newPassword    = isPasswordChange ? changepass.cha_newpassword : null
            };

            var updated = await _sharedHelpers.UpdateUser(reuser);
            if (updated == null)
                return Json(new { result = "Update failed. Please try again." });

            var activity = new CNET_V7_Domain.Domain.CommonSchema.ActivityDTO
            {
                Id                 = 0,
                Reference          = updated.Id,
                ActivityDefinition = 0,
                TimeStamp          = DateTime.UtcNow,
                Device             = null,
                User               = muser.Id,
                Pointer            = 1,
                Year               = DateTime.UtcNow.Year,
                Platform           = "web",
                Remark             = isUsernameChange ? (isPasswordChange ? "Username & Password changed" : "Username changed") : "Password changed"
            };
            await _sharedHelpers.CreateActivity(activity);

            return Json(new { result = "Saved Successfully !" });
        }

        #endregion
    }
}
