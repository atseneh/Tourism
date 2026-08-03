using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ministry_of_Tourism_pro.Common;
using Ministry_of_Tourism_pro.Models;
using Ministry_of_Tourism_pro.WebConstants;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using System.Security.Claims;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc.CommonTypes;

namespace Ministry_of_Tourism_pro.Controllers
{
    [Authorize]
    public class SystemAdminController : Controller
    {
        private readonly SharedHelpers _sharedHelpers;

        public SystemAdminController(SharedHelpers sharedHelpers)
        {
            _sharedHelpers = sharedHelpers;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _sharedHelpers.GetFilterData<List<VwUserPersonDTO>>("VwUserPerson");
            
            // Fetch roles from CNET_WebConstantes for the dropdown
            var roles = new List<ConsigneeUnitDTO>
            {
                new ConsigneeUnitDTO { Id = CNET_WebConstantes.SYSTEM_ADMINISTRATOR, Name = "SystemAdmin", Description = "System Administrator" },
                new ConsigneeUnitDTO { Id = CNET_WebConstantes.ADMINISTRATOR, Name = "HotelOwner", Description = "Hotel Administrator" },
                new ConsigneeUnitDTO { Id = CNET_WebConstantes.SUPERVISOR, Name = "Admin", Description = "Supervisor" },
                new ConsigneeUnitDTO { Id = CNET_WebConstantes.GENERAL_MANAGER, Name = "Commissioner", Description = "General Manager" }
            };
            ViewBag.Roles = roles;

            return View(users ?? new List<VwUserPersonDTO>());
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(VwUserPersonDTO model)
        {
            try
            {
                if (model.Id > 0)
                {
                    // Update User
                    var userUpdate = new UserUpdateDTO
                    {
                        userId = model.Id,
                        newUserName = model.UserName,
                        newPassword = !string.IsNullOrEmpty(model.Password) ? model.Password : null,
                        isActive = model.IsActive,
                        person = model.Person,
                        isAdmin = true,
                        changePassword = !string.IsNullOrEmpty(model.Password)
                    };
                    await _sharedHelpers.UpdateUser(userUpdate);

                    // Update Person Details (Consignee)
                    var person = await _sharedHelpers.GetConsigneeById(model.Person);
                    if (person != null)
                    {
                        person.FirstName = model.FirstName;
                        person.SecondName = model.SecondName;
                        person.IsActive = model.IsActive;
                        person.Remark = $"Email: {model.RoleName} | Phone: {model.Phone1}";
                        await _sharedHelpers.SendReqAsync<ConsigneeDTO, ConsigneeDTO>($"Consignee", HttpMethod.Put, person);
                    }

                    // Update Role (Delete existing and create new to ensure only one role)
                    if (!string.IsNullOrEmpty(model.RoleName))
                    {
                        var roleList = GetAvailableRoles();
                        var role = roleList.FirstOrDefault(r => (r.Description ?? r.Name) == model.RoleName);
                        if (role != null)
                        {
                            // 1. Find and delete existing role(s)
                            var currentMapper = await _sharedHelpers.GetUserRoleM(model.Id);
                            if (currentMapper != null)
                            {
                                await _sharedHelpers.SendReqAsync<object, object>($"UserRoleMapper/{currentMapper.Id}", HttpMethod.Delete);
                            }

                            // 2. Create new role
                            var newMapper = new UserRoleMapperDTO
                            {
                                Id = 0,
                                User = model.Id,
                                Role = role.Id,
                                ExpiryDate = DateTime.Now.AddYears(1),
                                Remark = "Branch"
                            };
                            await _sharedHelpers.CreateUserRoleMapper(newMapper);
                        }
                    }

                    TempData["SuccessMessage"] = "User updated successfully!";
                }
                else
                {
                    // Create logic
                    var consignee1 = new ConsigneeDTO
                    {
                        FirstName = model.FirstName,
                        SecondName = model.SecondName,
                        IsActive = true,
                        IsPerson = true,
                        Preference = CNET_WebConstantes.EMPLOYEE_CATEGORY,
                        Branch = CNET_WebConstantes.HARDCODED_BRANCH,
                        Remark = $"Email: {model.RoleName} | Phone: {model.Phone1}",
                        GslType = 26,
                        Code = Guid.NewGuid().ToString()
                    };
                    
                    var result2 = await _sharedHelpers.SendReqAsync<ConsigneeDTO, ConsigneeDTO>("Consignee", HttpMethod.Post, consignee1);
                    if (result2 != null)
                    {
                        var userDto_ = new UserDTO
                        {
                            UserName = model.UserName,
                            Remark = model.Phone1,
                            IsActive = true,
                            Password = string.IsNullOrEmpty(model.Password) ? "admin@123" : model.Password,
                            LoggedInStatus = 1389,
                            Person = result2.Id,
                            Salt = ""
                        };
                        
                        var userResp = await _sharedHelpers.CreateUser(userDto_);
                        if (userResp != null)
                        {
                            // Get Role ID from RoleName using hardcoded list
                            var roleList = GetAvailableRoles();
                            var role = roleList.FirstOrDefault(r => (r.Description ?? r.Name) == model.RoleName);
                            
                            // Delete any existing mapper (unlikely for new user but safe)
                            var currentMapper = await _sharedHelpers.GetUserRoleM(userResp.Id);
                            if (currentMapper != null)
                            {
                                await _sharedHelpers.SendReqAsync<object, object>($"UserRoleMapper/{currentMapper.Id}", HttpMethod.Delete);
                            }

                            var roleMapper = new UserRoleMapperDTO
                            {
                                Id = 0,
                                User = userResp.Id,
                                Role = role?.Id ?? CNET_WebConstantes.ADMINISTRATOR, 
                                ExpiryDate = DateTime.Now.AddYears(1),
                                Remark = "Branch"
                            };
                            await _sharedHelpers.CreateUserRoleMapper(roleMapper);
                        }
                    }
                    TempData["SuccessMessage"] = "User created successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error saving user: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        private List<ConsigneeUnitDTO> GetAvailableRoles()
        {
            return new List<ConsigneeUnitDTO>
            {
                new ConsigneeUnitDTO { Id = CNET_WebConstantes.SYSTEM_ADMINISTRATOR, Name = "SystemAdmin", Description = "System Administrator" },
                new ConsigneeUnitDTO { Id = CNET_WebConstantes.ADMINISTRATOR, Name = "HotelOwner", Description = "Hotel Administrator" },
                new ConsigneeUnitDTO { Id = CNET_WebConstantes.SUPERVISOR, Name = "Admin", Description = "Supervisor" },
                new ConsigneeUnitDTO { Id = CNET_WebConstantes.GENERAL_MANAGER, Name = "Commissioner", Description = "General Manager" }
            };
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _sharedHelpers.SendReqAsync<object, object>($"User/{id}", HttpMethod.Delete);
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting user: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

    }
}
