using System.ComponentModel.DataAnnotations;

namespace UserProfileApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Связь 1:1
        public UserProfile? Profile { get; set; }
    }
}
