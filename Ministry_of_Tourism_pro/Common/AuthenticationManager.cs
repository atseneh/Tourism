using Ministry_of_Tourism_pro.Common;
using Ministry_of_Tourism_pro.Models;
using Ministry_of_Tourism_pro.WebConstants;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Misc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Web;

namespace Ministry_of_Tourism_pro.Common
{
    public class AuthenticationManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly SharedHelpers _sharedHelpers;
        private UserDTO? _cachedUser;

        public AuthenticationManager(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            SharedHelpers sharedHelpers)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClientFactory.CreateClient("mainclient");
            _sharedHelpers = sharedHelpers;
        }

        public virtual async Task<validIdentificationReturn> identificationValid()
        {
            var validinfo = new validIdentificationReturn();
            var idCookie = CNET_WebConstantes.HARDCODED_TIN;
            
            if (!string.IsNullOrWhiteSpace(idCookie))
            {
                var myOrg = await _sharedHelpers.GetCompany();
                if (myOrg?.Tin == idCookie)
                {
                    var _branchlist = await _sharedHelpers.GetCompanyBranchsByTin(myOrg.Tin);
                    validinfo.CompanyTradeName = myOrg.FirstName;
                    validinfo.BranchList = _branchlist;
                    validinfo.tin = idCookie;
                    validinfo.isValid = true;
                    return validinfo;
                }
            }
            
            validinfo.isValid = false;
            return validinfo;
        }

        public async Task<ResponseModel<LoginResponse>> AuthenticateUser(string? userName, string? password, string? branch)
        {
            var _s = new ResponseModel<LoginResponse>();
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return _s;
            }

            if (userName.ToLower().Trim() == "cnet admin")
            {
                branch = "0";
            }

            var comp = await _sharedHelpers.GetCompany();
            if (comp == null) return _s;

            string cacheKey = $"GlobalParams_{userName}";
            ISession? session = _httpContextAccessor.HttpContext?.Session;

            string encodedPassword = HttpUtility.UrlEncode(password);
            var endpoint = "SysInitialize/authenticate";
            string queryString = $"?userName={userName}&password={encodedPassword}&tin={comp.Tin}&consigneeUnit={branch}";
            string requestUrl = $"{endpoint}{queryString}";

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            string juservalidation = await response.Content.ReadAsStringAsync();
            var userValidation = JsonConvert.DeserializeObject<ResponseModel<LoginResponse>>(juservalidation);

            if (userValidation?.Data != null && session != null)
            {
                var globalParams = new { 
                    navigatorList = userValidation.Data.navigatorList, 
                    personInfo = userValidation.Data.personInfo 
                };
                session.SetString(cacheKey, JsonConvert.SerializeObject(globalParams));
            }

            return userValidation ?? _s;
        }

        public virtual async Task SignIn(UserDTO user, bool isPersistent, string role)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(user.UserName))
                claims.Add(new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String, CNET_WebConstantes.ClaimsIssuer));

            if (!string.IsNullOrEmpty(user.Remark))
                claims.Add(new Claim(ClaimTypes.Email, user.Remark, ClaimValueTypes.String, CNET_WebConstantes.ClaimsIssuer));

            if (!string.IsNullOrEmpty(role))
                claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String, CNET_WebConstantes.ClaimsIssuer));

            var userIdentity = new ClaimsIdentity(claims, CNET_WebConstantes.CookieScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                IssuedUtc = DateTime.UtcNow
            };

            if (isPersistent)
            {
                authenticationProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14);
            }

            await _httpContextAccessor.HttpContext!.SignInAsync(CNET_WebConstantes.CookieScheme, userPrincipal, authenticationProperties);
            _cachedUser = user;
        }

        public virtual async Task<UserDTO?> GetAuthenticatedUser()
        {
            if (_cachedUser != null)
                return _cachedUser;

            var authenticateResult = await _httpContextAccessor.HttpContext!.AuthenticateAsync(CNET_WebConstantes.CookieScheme);
            if (!authenticateResult.Succeeded)
                return null;

            UserDTO? user = null;

            var usernameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name
                && claim.Issuer.Equals(CNET_WebConstantes.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
            
            if (usernameClaim != null)
            {
                user = await _sharedHelpers.GetUserByUserName(usernameClaim.Value);
            }

            var emailClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email
               && claim.Issuer.Equals(CNET_WebConstantes.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
            
            if (user != null && emailClaim != null)
            {
                user.Remark = emailClaim.Value;
            }

            _cachedUser = user;
            return _cachedUser;
        }

        public virtual async Task SignOut()
        {
            _cachedUser = null;

            if (_httpContextAccessor.HttpContext?.Session != null)
            {
                _httpContextAccessor.HttpContext.Session.Clear();
            }

            await _httpContextAccessor.HttpContext!.SignOutAsync(CNET_WebConstantes.CookieScheme);
        }
    }
}
