using System;

namespace Ministry_of_Tourism_pro.Models
{
    public class UserUpdateDTO
    {
        public int userId { get; set; }

        public string? oldUserName { get; set; }

        public string? newUserName { get; set; }

        public string? oldPassword { get; set; }

        public string? newPassword { get; set; }

        public bool? isActive { get; set; }

        public int? person { get; set; }

        public bool isAdmin { get; set; }

        public bool changePassword { get; set; }
    }
}
