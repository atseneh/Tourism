using System.ComponentModel.DataAnnotations;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;

namespace Ministry_of_Tourism_pro.Models
{
    public class ConsigneeListViewModel
    {
        public List<ConsigneeDTO> Organizations { get; set; } = new();
    }

    public class ConsigneeEditViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string TIN { get; set; } = string.Empty;
        
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public string? Remark { get; set; }
    }

    public class UserManagementViewModel
    {
        public int ConsigneeId { get; set; }
        public string ConsigneeName { get; set; } = string.Empty;
        public List<PersonUserDetail> Users { get; set; } = new();
    }

    public class PersonUserDetail
    {
        public int PersonId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSystemUserViewModel
    {
        public int ConsigneeId { get; set; }
        
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        public string UserName { get; set; } = string.Empty;
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public int RoleId { get; set; }
        
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    public class UserListViewModel
    {
        public List<UserWithRoleDetail> Users { get; set; } = new();
        public List<ConsigneeDTO> Organizations { get; set; } = new();
    }

    public class UserWithRoleDetail
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Remark { get; set; }
        public int PersonId { get; set; }
    }
}
