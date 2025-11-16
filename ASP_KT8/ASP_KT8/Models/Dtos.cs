using System.ComponentModel.DataAnnotations;
using ASP_KT8.Data;

namespace ASP_KT8.Models
{
    public class CreateUserDto
    {
        [Required, MinLength(3)]
        [RegularExpression(@"^[A-Za-zА-Яа-яЁё]+$", ErrorMessage = "Name must contain only letters!")]
        public string Username { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(8)]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[A-Za-z]).{8,}$", ErrorMessage = "Password must contain at least 8 characters and include only letters and numbers!")]
        public string Password { get; set; } = "";
    }

    public class UpdateUserDto
    {
        [Required, MinLength(3)]
        [RegularExpression(@"^[A-Za-zА-Яа-яЁё]+$", ErrorMessage = "Name must contain only letters!")]
        public string Username { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(8)]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[A-Za-z]).{8,}$", ErrorMessage = "Password must contain at least 8 characters and include only letters and numbers!")]
        public string Password { get; set; } = "";
    }
}
