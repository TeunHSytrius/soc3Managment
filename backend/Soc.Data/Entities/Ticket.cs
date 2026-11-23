using System.ComponentModel.DataAnnotations;
using Soc.Data.Entities.Enums;
namespace Soc.Data.Entities;

public class Ticket
{
    [Key] public int TicketId { get; set; }
    [Required][MaxLength(250)] public string Description { get; set; } = null!;
    [Required] public SlaStatus SlaStatus { get; set; } = SlaStatus.Pending;
    [Required] public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    [Required] public DateTimeOffset Deadline { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }

    public int? ResolvedById { get; set; }
    public Employee? ResolvedBy { get; set; }

    [Required] public int AlertId { get; set; }
    [Required] public Alert Alert { get; set; } = null!;

    public int? ResearchId { get; set; }
    public Research? Research { get; set; }
    public ICollection<Note> Notes { get; set; } = new List<Note>();
}
