using System.ComponentModel.DataAnnotations;

namespace Ministry_of_Tourism_pro.Models
{
    public class IdentificationViewModel
    {
        [Required]
        public string TIN { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? Branch { get; set; }
        
        public List<CNET_V7_Domain.Domain.ConsigneeSchema.ConsigneeUnitDTO>? BranchList { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        //[EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = "HotelOwner";

        [Display(Name = "TIN Number (For Hotel Owners)")]
        public string? TIN { get; set; }
    }
    public class PreRegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Legal Name / Establishment")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "TIN is required")]
        [Display(Name = "TIN Number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "TIN must be 10 digits")]
        public string TIN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;
    }
}
