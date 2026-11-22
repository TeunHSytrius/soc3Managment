namespace Soc.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class Note
{
    [Key]
    public int NoteId { get; set; }

    [Required] public string NoteContent { get; set; } = null!;
    [Required] public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Required] public int EmployeeId { get; set; }
    [Required] public Employee Employee { get; set; } = null!;

    [Required] public int TicketId { get; set; }
    [Required] public Ticket Ticket { get; set; } = null!;
}
