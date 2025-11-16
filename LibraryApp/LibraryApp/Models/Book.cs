using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public class Book
{
    public int Id { get; set; }

    [Required, StringLength(300)]
    public string Title { get; set; } = "";

    [Range(1400, 2100)]
    public int? Year { get; set; }

    // FK (каждая книга у одного автора)
    [Required]
    public int AuthorId { get; set; }

    public Author Author { get; set; } = default!;
}
