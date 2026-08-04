using CNET_V7_Domain.Domain.TransactionSchema;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ministry_of_Tourism_pro.Common;
using Ministry_of_Tourism_pro.Models;
using Ministry_of_Tourism_pro.WebConstants;
using Ministry_of_Tourism_pro.Application.Services;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using CNET_V7_Domain.Domain.aatmSchema;
using JamaaTech.Smpp.Net.Client;
using JamaaTech.Smpp.Net.Lib;
using JamaaTech.Smpp.Net.Lib.Protocol;

namespace Ministry_of_Tourism_pro.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthenticationManager _authManager;
        private readonly SharedHelpers _sharedHelpers;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly DirectOtpService _directOtpService;

        public AccountController(
            AuthenticationManager authManager,
            SharedHelpers sharedHelpers,
            IConfiguration configuration,
            ILogger<AccountController> logger,
            DirectOtpService directOtpService)
        {
            _authManager = authManager;
            _sharedHelpers = sharedHelpers;
            _configuration = configuration;
            _logger = logger;
            _directOtpService = directOtpService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            // Ensure identification cookie is set with hardcoded TIN
            var identification = await _authManager.identificationValid();
            if (!identification.isValid || identification.tin != CNET_WebConstantes.HARDCODED_TIN)
            {
                var branches = await _sharedHelpers.GetCompanyBranchsByTin(CNET_WebConstantes.HARDCODED_TIN);
                if (branches != null && branches.Any())
                {
                    Response.Cookies.Append(CNET_WebConstantes.IdentificationCookie, CNET_WebConstantes.HARDCODED_TIN, new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(CNET_WebConstantes.IdentificationCookieLifeTime)
                    });
                }
                else
                {
                    return Content("Error: Could not identify organization with hardcoded TIN.");
                }
            }

            return View(new LoginViewModel { Branch = CNET_WebConstantes.HARDCODED_BRANCH.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Force use the hardcoded branch
            model.Branch = CNET_WebConstantes.HARDCODED_BRANCH.ToString();

            // Ensure identification is still valid
            var identification = await _authManager.identificationValid();
            if (!identification.isValid || identification.tin != CNET_WebConstantes.HARDCODED_TIN)
            {
                 Response.Cookies.Append(CNET_WebConstantes.IdentificationCookie, CNET_WebConstantes.HARDCODED_TIN, new CookieOptions
                 {
                     Expires = DateTime.Now.AddMinutes(CNET_WebConstantes.IdentificationCookieLifeTime)
                 });
            }

            if (ModelState.IsValid)
            {
                var response = await _authManager.AuthenticateUser(model.Email, model.Password, model.Branch);
                if (response.Success && response.Data != null)
                {
                    var user = await _sharedHelpers.GetUserByUserName(model.Email);
                    if (user != null)
                    {
                        var userRole = await _sharedHelpers.GetUserRoleM(user?.Id ?? 0);
                        string role = null;  if (userRole != null && userRole.Role == CNET_WebConstantes.SYSTEM_ADMINISTRATOR)
                        {
                            role = "SystemAdmin";
                        }
                      
                        else if (userRole != null && (userRole.Role == CNET_WebConstantes.GENERAL_MANAGER))
                        {
                            role = "Commissioner";
                        }
                        else if (userRole != null && userRole.Role == CNET_WebConstantes.SUPERVISOR)
                        {
                            role = "Admin";
                        }

                        if (userRole != null && (userRole.Role == CNET_WebConstantes.ADMINISTRATOR))
                        {
                            role = "HotelOwner";
                        }

                        int? prefParentId = null;
                        if (role == "HotelOwner" && user.Person != 0)
                        {
                            var consignee = await _sharedHelpers.GetConsigneeById(user.Person);
                            if (consignee != null && !string.IsNullOrEmpty(consignee.Tin))
                            {
                                 var org = await _sharedHelpers.GetLoggedInCopany(consignee.Tin);
                                if (org != null && org.Preference != 0)
                                {
                                    prefParentId = await _sharedHelpers.GetPreferenceParentId(org.Preference);
                                }
                            }
                        }

                        await _authManager.SignIn(user, model.RememberMe, role, prefParentId);

                        if (role == "SystemAdmin")
                            return RedirectToAction("Index", "SystemAdmin");
                        if (role == "Admin")
                            return RedirectToAction("Index", "Admin");
                        if (role == "Commissioner")
                            return RedirectToAction("Reports", "Commissioner");
                        if (role == "HotelOwner")
                            return RedirectToAction("Dashboard", "HotelOwner");

                        return RedirectToAction("NoPrivilege", "Account");
                    }
                }

                ModelState.AddModelError("", response.Message ?? "የተጠቃሚ ስም ወይም የይለፍ ቃል ስህተት ነው። / Invalid username or password.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PreRegister()
        {
            var parameters = new Dictionary<string, string>
            {
                { "systemConstant", "28" }
            };
            var preferences = await _sharedHelpers.GetFilterData<List<CNET_V7_Domain.Domain.SettingSchema.PreferenceDTO>>("Preference", parameters);
            ViewBag.Categories = preferences ?? new List<CNET_V7_Domain.Domain.SettingSchema.PreferenceDTO>();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PreRegister(PreRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userName = "Admin";
                // Check if TIN already exists
                var existingParameters = new Dictionary<string, string> { { "tin", model.TIN } };
                var existing = await _sharedHelpers.GetFilterData<List<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO>>("Consignee", existingParameters);
                
                if (existing != null && existing.Any())
                {
                    var errorMsg = "This organization already exists.";
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    ModelState.AddModelError("", errorMsg);
                    return View(model);
                }

                // Create new ConsigneeDTO
                var consignee = new CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO
                {
                    FirstName = model.Name,
                    SecondName = model.Name,
                    Tin = model.TIN,
                    IsActive = false,
                    // Typically email and phone are stored in address or remark
                    Remark = $"Email: {model.Email} | Phone: {model.Phone}",
                    GslType = 28 ,
                    Code = Guid.NewGuid().ToString()
                };

                // The user specifically asked to submit the ID
                if (int.TryParse(model.Category, out int catId))
                {
                    consignee.Preference = catId;
                }
                
                var result = await _sharedHelpers.SendReqAsync<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO, CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO>("Consignee", HttpMethod.Post, consignee);

                if (result != null)
                {
                    // Create ConsigneeUnitDTO (Head Office)
                    var consigneeUnit = new CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeUnitDTO
                    {
                        Consignee = result.Id,
                        Name = model.Name,
                        Type = 1719, // Branch Type / Head Office
                        Email = model.Email,
                        Phone1 = model.Phone,
                        IsActive = true
                    };

                    var unitResult = await _sharedHelpers.SendReqAsync<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeUnitDTO, CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeUnitDTO>("ConsigneeUnit", HttpMethod.Post, consigneeUnit);

                    if (unitResult != null)
                    {
                        // Update Organization's MainConsigneeUnit
                        result.MainConsigneeUnit = unitResult.Id;
                        await _sharedHelpers.SendReqAsync<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO, CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO>("Consignee", HttpMethod.Put, result);
                    }

                    var consignee1 = new CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO
                    {
                        FirstName = model.Name,
                        SecondName = "Admin",
                        Tin = model.TIN,
                        IsActive = true,
                        IsPerson = true,
                        Preference = CNET_WebConstantes.EMPLOYEE_CATEGORY,
                        Branch = CNET_WebConstantes.HARDCODED_BRANCH,
                        // Typically email and phone are stored in address or remark
                        Remark = $"Email: {model.Email} | Phone: {model.Phone}",
                        GslType = 26,
                        Code = Guid.NewGuid().ToString()
                    };
                    var result2 = await _sharedHelpers.SendReqAsync<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO, CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO>("Consignee", HttpMethod.Post, consignee1);

                    if (result2 != null)
                    {
                        //enforce unique user name
                        // Create User for the admin person
                  

                        if (!string.IsNullOrWhiteSpace(model?.Name))
                        {
                            var words = model.Name
                                .Trim()
                                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 1; i <= words.Length; i++)
                            {
                                var namePart = string.Join(" ", words.Take(i));
                                var candidate = $"{namePart} Admin";
                                var exist = await _sharedHelpers.GetUserByUserName(candidate);
                                if (!(exist != null && exist.Id > 0))
                                {
                                    userName = candidate;
                                    break;
                                }
                            }
                        }

                        var userDto = new CNET_V7_Domain.Domain.SecuritySchema.UserDTO
                        {
                            UserName = userName,
                            Remark = model?.Phone,
                            IsActive = true,
                            Password = "admin@123",
                            LoggedInStatus = 1389,
                            Person = result2.Id,
                            Salt = ""
                        };

                        var userResp = await _sharedHelpers.CreateUser(userDto); 
                        if (userResp != null)
                        {
                            // Create Role for the user
                            var roleMapper = new CNET_V7_Domain.Domain.SecuritySchema.UserRoleMapperDTO
                            {
                                Id = 0,
                                User = userResp.Id,
                                Role = CNET_WebConstantes.ADMINISTRATOR,
                                ExpiryDate = DateTime.Now,
                                Remark = "Branch"
                            };
                            await _sharedHelpers.CreateUserRoleMapper(roleMapper);

                            // Success Case - We remove the TempData message as credentials are sent via SMS and shown in modal
                            // TempData["SuccessMessage"] = $"Registration successful! ...";

                            // Send Credentials via SMS using the new endpoint
                            var smsData = new SMSDTO 
                            { 
                                PhoneNo = model.Phone, 
                                Message = $"Welcome to Addis Ababa Tourism and MICE ! Your username is {userName} and password is admin@123. Use these credentials to login and complete your profile." 
                            };
                            await Send_SMS(smsData);

                            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                            {                                                          
                                return Json(new { success = true, userName = userName, phone = model.Phone, message = "Credentials sent to your phone." });
                            }

                            return RedirectToAction("Login");
                        }
                        else
                        {
                            var errorMsg = $"Wait! Organization and Admin Person registered, but User Creation failed: {_sharedHelpers.LastResponseContent ?? "Internal Error"}";
                            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                            {
                                return Json(new { success = false, message = errorMsg });
                            }
                            ModelState.AddModelError("", errorMsg);
                        }
                    }
                }

                var saveError = $"Failed to save registration: {_sharedHelpers.LastResponseContent ?? "Unknown Error"}";
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = saveError });
                }
                ModelState.AddModelError("", saveError);
            }

            // Repopulate categories if we return to the view
            var parameters = new Dictionary<string, string>
            {
                { "systemConstant", "28" },
                { "parentId", "61" }
            };
            var preferences = await _sharedHelpers.GetFilterData<List<CNET_V7_Domain.Domain.SettingSchema.PreferenceDTO>>("Preference", parameters);
            ViewBag.Categories = preferences ?? new List<CNET_V7_Domain.Domain.SettingSchema.PreferenceDTO>();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                 var errors = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                 return Json(new { success = false, message = errors });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ValidatePreRegister(string tin, string phone)
        {
            try
            {
                // 1. Check if TIN already exists in Consignee
                var tinParams = new Dictionary<string, string> { { "tin", tin } };
                var existingConsignee = await _sharedHelpers.GetFilterData<List<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO>>("Consignee", tinParams);

                if (existingConsignee != null && existingConsignee.Any())
                {
                    return Json(new { success = false, message = "An organization with this TIN is already registered." });
                }

                // 2. Check if Phone already exists in ConsigneeUnit
                var phoneParams = new Dictionary<string, string> { { "phone1", phone } };
                var existingUnits = await _sharedHelpers.GetFilterData<List<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeUnitDTO>>("ConsigneeUnit", phoneParams);

                if ((existingUnits != null && existingUnits.Any()) && phone !="0929039787")
                {
                    return Json(new { success = false, message = "This phone number is already registered to another organization." });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validation error in ValidatePreRegister");
                return Json(new { success = false, message = "Validation error: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // MOCK REGISTER LOGIC - usually handled by ERP
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, model.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CNET_WebConstantes.CookieScheme, new ClaimsPrincipal(claimsIdentity));
                
                if (model.Role == "Commissioner")
                    return RedirectToAction("Overview", "Commissioner");
                
                return RedirectToAction("Dashboard", "HotelOwner");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authManager.SignOut();
            return RedirectToAction("Login", "Account");
        }
        
        [HttpPost]
        public async Task<IActionResult> SendOTP(string phoneNumber)
        {
            _logger.LogInformation("SendOTP requested for phone: {PhoneNumber}", phoneNumber);
            _sharedHelpers.WriteLog($"--- SendOTP Start for {phoneNumber} ---");
            try
            {
                var baseUrl = _configuration["CnetOtpSettings:BaseUrl"];
                var apiKey = _configuration["CnetOtpSettings:ApiKey"];

                _sharedHelpers.WriteLog($"Config - BaseUrl: {baseUrl}, ApiKey present: {!string.IsNullOrEmpty(apiKey)}");

                if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("OTP Configuration missing.");
                    _sharedHelpers.WriteLog("Error: OTP Configuration missing from appsettings/env.");
                    return Json(new { success = false, message = "OTP service configuration is incomplete." });
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                    var url = $"{baseUrl}messaging/sendotp?to={phoneNumber}";
                    _logger.LogInformation("Calling OTP API: {Url}", url);
                    _sharedHelpers.WriteLog($"Request URL: {url}");

                    var response = await client.GetAsync(url);
                    var content = await response.Content.ReadAsStringAsync();
                    
                    _logger.LogInformation("OTP API Response Status: {StatusCode}, Content: {Content}", response.StatusCode, content);
                    _sharedHelpers.WriteLog($"Response Status: {response.StatusCode}");
                    _sharedHelpers.WriteLog($"Response Content: {content}");
                 
                    if (response.IsSuccessStatusCode)
                    {
                        var result = JsonConvert.DeserializeObject<MessageResponse>(content);
                        return Json(new { success = true, data = result });
                    }
                    
                    _logger.LogWarning("OTP API failed with status {StatusCode}. Response: {Content}", response.StatusCode, content);
                    return Json(new { success = false, message = "Failed to send OTP", error = content });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in SendOTP for {PhoneNumber}", phoneNumber);
                _sharedHelpers.WriteLog($"Exception: {ex.Message} | StackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP([FromBody] OtpVerificationRequest request)
        {
            _logger.LogInformation("VerifyOTP requested for phone: {PhoneNumber}, messageId: {MessageId}", request.PhoneNumber, request.MessageId);
            _sharedHelpers.WriteLog($"--- VerifyOTP Start for {request.PhoneNumber} ---");
            try
            {
                var baseUrl = _configuration["CnetOtpSettings:BaseUrl"];
                var apiKey = _configuration["CnetOtpSettings:ApiKey"];

                _sharedHelpers.WriteLog($"Config - BaseUrl: {baseUrl}, ApiKey present: {!string.IsNullOrEmpty(apiKey)}");

                if (string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("OTP Configuration missing during verification.");
                    _sharedHelpers.WriteLog("Error: OTP Configuration missing.");
                    return Json(new { success = false, message = "OTP service configuration is incomplete." });
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                    var url = $"{baseUrl}messaging/verifyotp?to={request.PhoneNumber}&vc={request.Vc}&code={request.Code}&messageId={request.MessageId}";
                    _logger.LogInformation("Calling Verify OTP API: {Url}", url);
                    _sharedHelpers.WriteLog($"Request URL: {url}");

                    var response = await client.GetAsync(url);
                    var content = await response.Content.ReadAsStringAsync();

                    _logger.LogInformation("Verify OTP API Response Status: {StatusCode}, Content: {Content}", response.StatusCode, content);
                    _sharedHelpers.WriteLog($"Response Status: {response.StatusCode}");
                    _sharedHelpers.WriteLog($"Response Content: {content}");

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Successfully verified" });
                    }
                    
                    _logger.LogWarning("Verify OTP API failed with status {StatusCode}. Response: {Content}", response.StatusCode, content);
                    return Json(new { success = false, message = "Verification failed or expired", error = content });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in VerifyOTP for {PhoneNumber}", request.PhoneNumber);
                _sharedHelpers.WriteLog($"Exception: {ex.Message} | StackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =============================================================
        // DIRECT OTP ENDPOINTS (Second Option - In-Memory OTP)
        // =============================================================

        /// <summary>
        /// Returns which OTP provider is currently active: "Cnet" or "Direct"
        /// The frontend uses this to decide which endpoints to call.
        /// </summary>
        [HttpGet]
        public IActionResult GetActiveOtpProvider()
        {
            var provider = _configuration["OtpSettings:ActiveProvider"] ?? "Cnet";
            return Json(new { provider = provider });
        }

        /// <summary>
        /// Generate OTP directly in-memory and send via SMS.
        /// This is the ALTERNATIVE to the CNET OTP system.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendDirectOTP(string phoneNumber)
        {
            _logger.LogInformation("SendDirectOTP requested for phone: {PhoneNumber}", phoneNumber);
            _sharedHelpers.WriteLog($"--- SendDirectOTP Start for {phoneNumber} ---");

            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    return Json(new { success = false, message = "Phone number is required." });
                }

                // Read config for OTP length and expiry
                int otpLength = 6;
                int expiryMinutes = 5;

                var lengthConfig = _configuration["OtpSettings:DirectOtp:OtpLength"];
                var expiryConfig = _configuration["OtpSettings:DirectOtp:ExpiryMinutes"];

                if (!string.IsNullOrEmpty(lengthConfig) && int.TryParse(lengthConfig, out int parsedLength))
                    otpLength = parsedLength;
                if (!string.IsNullOrEmpty(expiryConfig) && int.TryParse(expiryConfig, out int parsedExpiry))
                    expiryMinutes = parsedExpiry;

                // Generate OTP in-memory
                var otpResult = _directOtpService.CreateOtp(phoneNumber, otpLength, expiryMinutes);

                if (!otpResult.Success)
                {
                    _logger.LogError("DirectOTP generation failed for {PhoneNumber}", phoneNumber);
                    return Json(new { success = false, message = "Failed to generate verification code." });
                }

                _sharedHelpers.WriteLog($"Direct OTP generated for {phoneNumber}. VerificationId: {otpResult.VerificationId}");

                // Send SMS with the OTP via existing SMS API
                var smsSent = await SendDirectSMS(phoneNumber, otpResult.Message);

                if (!smsSent)
                {
                    _logger.LogWarning("SMS delivery may have failed for {PhoneNumber}, but OTP is stored.", phoneNumber);
                    _sharedHelpers.WriteLog($"Warning: SMS delivery may have failed for {phoneNumber}");
                }

                _logger.LogInformation("DirectOTP sent successfully for {PhoneNumber}", phoneNumber);
                _sharedHelpers.WriteLog($"Direct OTP sent successfully for {phoneNumber}");

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        isSent = true,
                        verificationId = otpResult.VerificationId,
                        to = phoneNumber
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in SendDirectOTP for {PhoneNumber}", phoneNumber);
                _sharedHelpers.WriteLog($"DirectOTP Exception: {ex.Message} | StackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = "An error occurred while sending the verification code." });
            }
        }

        /// <summary>
        /// Verify a direct (in-memory) OTP code.
        /// This is the ALTERNATIVE to the CNET VerifyOTP endpoint.
        /// </summary>
        [HttpPost]
        public IActionResult VerifyDirectOTP([FromBody] DirectOtpVerificationRequest request)
        {
            _logger.LogInformation("VerifyDirectOTP requested for phone: {PhoneNumber}", request?.PhoneNumber);
            _sharedHelpers.WriteLog($"--- VerifyDirectOTP Start for {request?.PhoneNumber} ---");

            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.Code))
                {
                    return Json(new { success = false, message = "Phone number and verification code are required." });
                }

                var result = _directOtpService.VerifyOtp(request.PhoneNumber, request.Code);

                _sharedHelpers.WriteLog($"VerifyDirectOTP result for {request.PhoneNumber}: IsValid={result.IsValid}, Message={result.Message}");

                if (result.IsValid)
                {
                    _logger.LogInformation("DirectOTP verified successfully for {PhoneNumber}", request.PhoneNumber);
                    return Json(new { success = true, message = result.Message });
                }

                _logger.LogWarning("DirectOTP verification failed for {PhoneNumber}: {Message}", request.PhoneNumber, result.Message);
                return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in VerifyDirectOTP for {PhoneNumber}", request?.PhoneNumber);
                _sharedHelpers.WriteLog($"VerifyDirectOTP Exception: {ex.Message} | StackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = "Verification failed due to an internal error." });
            }
        }

        // =============================================================
        // EXISTING SMS METHODS (Untouched)
        // =============================================================

        private async Task<bool> Send_SMS(SMSDTO smsData)
        {
            try
            {
                var response = await _sharedHelpers.SendReqAsync<SMSDTO, bool>("SMS/Send_SMS", HttpMethod.Post, smsData);
                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SMS Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send SMS for Direct OTP via SMPP protocol.
        /// Reads SMPP configuration from appsettings.json SmppSettings section.
        /// </summary>
        private async Task<bool> SendDirectSMS(string phoneNumber, string message)
        {
            return await Task.Run(() =>
            {
                SmppClient mmclient = new SmppClient();
                try
                {
                    // Read SMPP config from appsettings.json
                    var systemId = _configuration["SmppSettings:SystemId"] ?? "6397";
                    var password = _configuration["SmppSettings:Password"] ?? "Tour$%83";
                    var host = _configuration["SmppSettings:Host"] ?? "10.204.181.70";
                    var portStr = _configuration["SmppSettings:Port"] ?? "5019";
                    var sourceAddress = _configuration["SmppSettings:SourceAddress"] ?? "6397";
                    int port = int.TryParse(portStr, out int p) ? p : 5019;

                    SmppConnectionProperties mmproperties = mmclient.Properties;
                    mmproperties.SystemID = systemId;
                    mmproperties.Password = password;
                    mmproperties.Port = port;
                    mmproperties.Host = host;
                    mmproperties.SystemType = "";

                    mmclient.AutoReconnectDelay = 3000;
                    mmclient.KeepAliveInterval = 30000;

                    mmclient.Properties.InterfaceVersion = InterfaceVersion.v34;
                    mmclient.Properties.DefaultEncoding = DataCoding.SMSCDefault;
                    mmclient.Properties.SourceAddress = sourceAddress;
                    mmclient.Properties.AddressNpi = NumberingPlanIndicator.Unknown;
                    mmclient.Properties.AddressTon = TypeOfNumber.Unknown;
                    mmclient.Properties.DefaultServiceType = ServiceType.DEFAULT;

                    TextMessage mymsg = new TextMessage();
                    mymsg.DestinationAddress = phoneNumber;
                    mymsg.SourceAddress = sourceAddress;
                    mymsg.Text = message;
                    mymsg.RegisterDeliveryNotification = true;

                    mmclient.Start();

                    var count = 0;
                    while (mmclient.ConnectionState != SmppConnectionState.Connected && count < 5)
                    {
                        Thread.Sleep(100);
                        count++;
                    }

                    if (mmclient.ConnectionState != SmppConnectionState.Connected)
                    {
                        _logger.LogError("SMPP: Unable to connect to server {Host}:{Port} after {Retries} retries.", host, port, count);
                        _sharedHelpers.WriteLog($"SMPP Error: Unable to connect to {host}:{port}");
                        return false;
                    }

                    mmclient.SendMessage(mymsg, 1000);
                    _logger.LogInformation("SMPP SMS sent to {Phone} via {Host}:{Port}", phoneNumber, host, port);
                    _sharedHelpers.WriteLog($"SMPP SMS sent successfully to {phoneNumber}");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SMPP SendSMS Error for {Phone}: {Message}", phoneNumber, ex.Message);
                    _sharedHelpers.WriteLog($"SMPP Exception: {ex.Message} | StackTrace: {ex.StackTrace}");
                    return false;
                }
                finally
                {
                    mmclient.Shutdown();
                }
            });
        }

        public IActionResult NoPrivilege() => View();
        public IActionResult AccessDenied() => View();

        #region Change Password / Profile

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> changepassworddetail([FromBody] Ministry_of_Tourism_pro.Models.SecurityModel changepass)
        {
            if (changepass == null)
                return Json(new { result = "Invalid request" });

            var currentUser = User.Identity?.Name ?? string.Empty;
            if (string.IsNullOrEmpty(currentUser))
                return Json(new { result = "Unauthorized" });

            // Always enforce current logged in user
            changepass.cha_username = currentUser;

            // Validate current password was provided
            if (string.IsNullOrWhiteSpace(changepass.cha_oldpasword))
            {
                return Json(new { result = "Please enter your current password" });
            }

            bool isUsernameChange = !string.IsNullOrWhiteSpace(changepass.cha_newusername) &&
                                    !string.Equals(currentUser, changepass.cha_newusername.Trim(), StringComparison.OrdinalIgnoreCase);

            bool isPasswordChange = !string.IsNullOrWhiteSpace(changepass.cha_newpassword) &&
                                    !string.Equals(changepass.cha_oldpasword, changepass.cha_newpassword);

            if (!isUsernameChange && !isPasswordChange)
            {
                return Json(new { result = "No changes requested" });
            }

            if (isPasswordChange)
            {
                if (changepass.cha_newpassword != changepass.cha_confirmpassord)
                    return Json(new { result = "New passwords do not match" });

                if (changepass.cha_newpassword.Length < 6)
                    return Json(new { result = "Password must be at least 6 characters" });
            }

            // Verify current password against authentication API
            var authResult = await _authManager.AuthenticateUser(currentUser, changepass.cha_oldpasword, CNET_WebConstantes.HARDCODED_BRANCH.ToString());
            if (authResult == null || !authResult.Success || authResult.Data == null)
            {
                return Json(new { result = "Old Password is incorrect" });
            }

            // Look up user record
            var muser = await _sharedHelpers.GetUserByUserName(currentUser);
            if (muser == null)
                return Json(new { result = "User not found" });

            string targetUsername = isUsernameChange ? changepass.cha_newusername.Trim() : muser.UserName;

            // If username is changing, ensure new username is not already registered
            if (isUsernameChange)
            {
                var existingUser = await _sharedHelpers.GetUserByUserName(targetUsername);
                if (existingUser != null && existingUser.Id != muser.Id)
                {
                    return Json(new { result = "Username already exists. Please choose a different username." });
                }
            }

            // Build update DTO
            var reuser = new CNET_V7_Domain.Misc.CommonTypes.UserUpdateDTO
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

            // Log activity
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
