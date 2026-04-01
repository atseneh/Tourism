using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ministry_of_Tourism_pro.Common;
using Ministry_of_Tourism_pro.Models;
using Ministry_of_Tourism_pro.WebConstants;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using System.Security.Claims;
using CNET_V7_Domain.Domain.ViewSchema;

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
            return View(users ?? new List<VwUserPersonDTO>());
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(VwUserPersonDTO model)
        {
            try
            {
                if (model.Id > 0 && model.Person > 0)
                {
                    // Update
                    //var userDto = new UserUpdateDTO
                    //{
                    //    Id = model.Id,
                    //    UserName = model.UserName,
                    //    Password = model.Password,
                    //    IsActive = model.IsActive,
                    //    Person = model.Person,
                    //    Remark = model.Remark
                    //};
                    //await _sharedHelpers.UpdateUser(userDto);
                    TempData["SuccessMessage"] = "User updated successfully!";
                }
                else
                {
                    var consignee1 = new CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO
                    {
                        FirstName = model.FirstName,
                        SecondName = model.SecondName,
                        //Tin = model.TIN,
                        IsActive = true,
                        IsPerson = true,
                        Preference = CNET_WebConstantes.EMPLOYEE_CATEGORY,
                        Branch = CNET_WebConstantes.HARDCODED_BRANCH,
                        // Typically email and phone are stored in address or remark
                        Remark = $"Email: {model.RoleName} | Phone: {model.Phone1}",
                        GslType = 26,
                        Code = Guid.NewGuid().ToString()
                    };
                    var result2 = await _sharedHelpers.SendReqAsync<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO, CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeDTO>("Consignee", HttpMethod.Post, consignee1);
                    if (result2 != null)
                    {
                        // Create User for the admin person
                        var userDto_ = new CNET_V7_Domain.Domain.SecuritySchema.UserDTO
                        {
                            UserName = model.UserName,
                            Remark = model.Phone1,
                            IsActive = true,
                            Password = "admin@123",
                            LoggedInStatus = 1389,
                            Person = result2.Id,
                            Salt = ""
                        };
                        var userResp = await _sharedHelpers.CreateUser(userDto_);
                        if (userResp != null)
                        {
                            // Create Role for the user
                            var roleMapper = new CNET_V7_Domain.Domain.SecuritySchema.UserRoleMapperDTO
                            {
                                Id = 0,
                                User = userResp.Id,
                                Role = 106,
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
                    // Create
                 
                    TempData["SuccessMessage"] = "User created successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error saving user: " + ex.Message;
            }
            return RedirectToAction("Index");
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
