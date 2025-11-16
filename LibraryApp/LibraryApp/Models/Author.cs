using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public class Author
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = "";

    [StringLength(400)]
    public string? Bio { get; set; }

    // Навигация 1..*
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
