using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ministry_of_Tourism_pro.Models
{
    public class LoginModel
    {
        [DataType(DataType.Text)]
        [DisplayName("Username")]
        public string? Username { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string? Password { get; set; }

        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }

        [Required(ErrorMessage = "Branch is required!")]
        public string? Branch { get; set; }
        public string? _tin { get; set; }
        public validIdentificationReturn? ValidID { get; set; } = new validIdentificationReturn();
    }

    public class validIdentificationReturn
    {
        public string? tin { get; set; }
        public bool isValid { get; set; }
        public string? CompanyTradeName { get; set; }
        public List<ConsigneeUnitDTO>? BranchList { get; set; }
    }

    public class LicenseDisplayDTO
    {
        public string System { get; set; } = string.Empty;
        public int RemainingDays { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class VerifyIdModel
    {
        [DisplayName("Identification No.")]
        public string? myId { get; set; }
        [DisplayName("Remember")]
        public bool remember { get; set; }
    }

    public class ChangePasswordModel
    {
        public string Username { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? Branch { get; set; }
    }
}
