using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfileApp.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Bio { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        // Внешний ключ 1:1
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public User? User { get; set; }
    }
}
