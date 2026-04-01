using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Ministry_of_Tourism_pro.Common;
using Ministry_of_Tourism_pro.Domain.Entities;
using Ministry_of_Tourism_pro.Models;
using Ministry_of_Tourism_pro.WebConstants;
using System.Data;
using System.Security.Claims;

namespace Ministry_of_Tourism_pro.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthenticationManager _authManager;
        private readonly SharedHelpers _sharedHelpers;


        public AccountController(AuthenticationManager authManager, SharedHelpers sharedHelpers)
        {
            _authManager = authManager;
            _sharedHelpers = sharedHelpers;
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

                        await _authManager.SignIn(user, model.RememberMe, role);

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
                    ModelState.AddModelError("", "This organization already exists.");
                    return View(model);
                }

                // Create new ConsigneeDTO
                var consignee = new CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO
                {
                    FirstName = model.Name,
                    SecondName = model.Name,
                    Tin = model.TIN,
                    IsActive = true,
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
                        }
                        else
                        {
                            TempData["ErrorMessage"] = $"Wait! Organization and Admin Person registered, but User Creation failed: {_sharedHelpers.LastResponseContent}";
                        }
                    }

                    TempData["SuccessMessage"] = $"Registration successful! Your default username is {userName}Admin and password is admin@123. Warning: please change your password and user name. You can now wait for approval.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", $"Failed to save registration: {_sharedHelpers.LastResponseContent ?? "Unknown Error"}");
            }

            // Repopulate categories if we return to the view
            var parameters = new Dictionary<string, string>
            {
                { "systemConstant", "28" },
                { "parentId", "61" }
            };
            var preferences = await _sharedHelpers.GetFilterData<List<CNET_V7_Domain.Domain.SettingSchema.PreferenceDTO>>("Preference", parameters);
            ViewBag.Categories = preferences ?? new List<CNET_V7_Domain.Domain.SettingSchema.PreferenceDTO>();

            return View(model);
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

        public IActionResult NoPrivilege() => View();
        public IActionResult AccessDenied() => View();
    }
}
