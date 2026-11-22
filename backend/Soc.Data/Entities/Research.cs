namespace Soc.Data.Entities;
using Soc.Data.Entities.Enums;
using System.ComponentModel.DataAnnotations;

public class Research
{
    [Key]
    public int ResearchId { get; set; }
    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    [Required]
    public int AlertID { get; set; }
    public Alert Alert { get; set; } = null!;
    [Required]
    public ConfidentialityLevel ConfidentialityLevel { get; set; }
    [Required]
    public PriorityLevel PriorityLevel { get; set; }

    public ICollection<Evidence> Evidences { get; set; } = new List<Evidence>();
    [Required]
    public string ResearchDescription { get; set; }

    //werknemer die de research verwerkt
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Required] public int TicketId { get; set; }

    [Required] public Ticket Ticket { get; set; } = null!;

}
