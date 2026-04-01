using Ministry_of_Tourism_pro.Models;
using Ministry_of_Tourism_pro.WebConstants;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.ViewSchema;
using CNET_V7_Domain.Misc;
using CNET_V7_Domain.Misc.CommonTypes;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Ministry_of_Tourism_pro.Common
{
    public class SharedHelpers
    {
        private readonly HttpClient _httpClient;

        public SharedHelpers(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("mainclient");
        }

        public string? LastResponseContent { get; private set; }

        public async Task<T?> SendReqAsync<TReq, T>(string endpoint, HttpMethod method, TReq? body = default)
        {
            try
            {
                LastResponseContent = null;
                HttpRequestMessage request = new HttpRequestMessage(method, endpoint);
                
                if (body != null)
                {
                    var x = JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(x, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);
                LastResponseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(LastResponseContent);
                }
                
                Debug.WriteLine($"API Error ({response.StatusCode}): {LastResponseContent}");
                return default;
            }
            catch (Exception ex)
            {
                LastResponseContent = ex.Message;
                Debug.WriteLine($"Error in SendReqAsync: {ex.Message}");
                return default;
            }
        }

        public async Task<ConsigneeDTO?> GetCompany()
        {
            var response = await _httpClient.GetAsync("Consignee/filter?gslType=1");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(content);
                return result?.FirstOrDefault();
            }
            return null;
        }
        public async Task<ConsigneeDTO?> GetLoggedInCopany(string? _tin)
        {
            var response = await _httpClient.GetAsync("Consignee/filter?gslType=28&tin=" + _tin);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(content);
                return result?.FirstOrDefault();
            }
            return null;
        }
        public async Task<ConsigneeDTO?> GetConsigneeById(int? id)
        {
            var response = await _httpClient.GetAsync("Consignee/filter?id=" + id.ToString());
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<ConsigneeDTO>>(content);
                return result?.FirstOrDefault();
            }
            return null;
        }

        public virtual async Task<List<ConsigneeUnitDTO>?> GetCompanyBranchsByTin(string? _tin)
        {
            var _CompanyBranchs = new List<ConsigneeUnitDTO>();

            var response = await _httpClient.GetAsync("SysInitialize?deviceName=web&tin=" + _tin + "&platform=0&isWeb=true");
            if (!response.IsSuccessStatusCode)
                return null;

            var jsysDto = await response.Content.ReadAsStringAsync();
            var sysDto_ = JsonConvert.DeserializeObject<ResponseModel<SystemInitDTO>>(jsysDto);

            _CompanyBranchs = sysDto_.Data != null ? sysDto_?.Data.branches?.ToList() : null;

            return _CompanyBranchs;
        }

        public async Task<UserDTO?> GetUserByUserName(string username)
        {
            var response = await _httpClient.GetAsync($"User/filter?userName={username}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<UserDTO>>(content);
                return result?.FirstOrDefault();
            }
            return null;
        }
        public async Task<UserRoleMapperDTO?> GetUserRoleM(int _userID)
        {
            var xrolemapper = new UserRoleMapperDTO();
            var response = await _httpClient.GetAsync("UserRoleMapper/filter?user=" + _userID);
            if (!response.IsSuccessStatusCode)
                return null;

            var juser = await response.Content.ReadAsStringAsync();
            var userrolemapper = JsonConvert.DeserializeObject<List<UserRoleMapperDTO>>(juser);

            xrolemapper = userrolemapper != null && userrolemapper.Count > 0 ? userrolemapper.FirstOrDefault() : null;

            return xrolemapper;
        }
        public virtual async Task<UserDTO?> UpdateUser(UserUpdateDTO userDto)
        {
            var response = await _httpClient.PostAsJsonAsync("CommonLibrary/update_user", userDto);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CNET_V7_Domain.Misc.ResponseModel<UserDTO>>(content);
                return result?.Data;
            }
            return null;
        }

        public virtual async Task<UserDTO?> CreateUser(UserDTO _userDTO)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("CommonLibrary/create_user", _userDTO);
                LastResponseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonConvert.DeserializeObject<CNET_V7_Domain.Misc.ResponseModel<UserDTO>>(LastResponseContent);
                    return data?.Data;
                }
                else
                {
                    Debug.WriteLine($"Error Calling Web Api (CommonLibrary/create_user): {response.StatusCode} - {LastResponseContent}");
                }
            }
            catch (Exception e)
            {
                LastResponseContent = e.Message;
                Debug.WriteLine($"Exception in CreateUser: {e.Message}");
            }
            return null;
        }

        public virtual async Task<UserRoleMapperDTO?> CreateUserRoleMapper(UserRoleMapperDTO _mapper)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("UserRoleMapper", _mapper);
                if (response.IsSuccessStatusCode)
                {
                    var jsysconstDto_ = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<UserRoleMapperDTO>(jsysconstDto_);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error Calling Web Api (UserRoleMapper): {response.StatusCode} - {content}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception in CreateUserRoleMapper: {e.Message}");
            }
            return null;
        }

        public string Decrypt(string encryptedText)
        {
            try
            {
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] cipherBytes = Convert.FromBase64String(encryptedText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey,
                        new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        encryptedText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return encryptedText;
            }
            catch { return encryptedText; }
        }

        public async Task<List<ActivityDefinitionDTO>> GetActivityDefinitionByDesc(int desc)
        {
            var response = await _httpClient.GetAsync($"ActivityDefinition/filter?description={desc}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ActivityDefinitionDTO>>(content) ?? new List<ActivityDefinitionDTO>();
            }
            return new List<ActivityDefinitionDTO>();
        }

        public async Task<ActivityDefinitionDTO?> CreateActivityDefinition(ActivityDefinitionDTO definition)
        {
            return await SendReqAsync<ActivityDefinitionDTO, ActivityDefinitionDTO>("ActivityDefinition", HttpMethod.Post, definition);
        }

        public async Task<ActivityDTO?> CreateActivity(ActivityDTO activity)
        {
            return await SendReqAsync<ActivityDTO, ActivityDTO>("Activity", HttpMethod.Post, activity);
        }

        public async Task<T?> GetFilterData<T>(string table, Dictionary<string, string>? parameters = null)
        {
            var queryString = "";
            if (parameters != null && parameters.Count > 0)
            {
                queryString = "?" + string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
            }
            var response = await _httpClient.GetAsync($"{table}/filter{queryString}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            return default;
        }

        public async Task<T?> GetFilterDynamic<T>(string table, Dictionary<string, string>? parameters = null)
        {
            var queryString = "";
            if (parameters != null && parameters.Count > 0)
            {
                queryString = "?" + string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
            }
            var response = await _httpClient.GetAsync($"{table}/dynamic{queryString}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            return default;
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(url, content);
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString, Encoding.UTF8, "application/json");
            return await httpClient.PutAsync(url, content);
        }
    }
}
