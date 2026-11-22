using Soc.Data.Entities;
using System.ComponentModel.DataAnnotations;
using Soc.Data.Entities.Enums;

public class Alert
{
    [Key]
    public int AlertId { get; set; }
    [Required]
    public String AlertName { get; set; }
    [Required]
    [MaxLength(300)]
    public String AlertDescription { get; set; }
    [Required]
    public String AlertType { get; set; }
    [Required]
    public DateTimeOffset AlertCreatedAt { get; set; } = DateTimeOffset.UtcNow;
    [Required]
    public ConfidentialityLevel ConfidentialityLevel { get; set; }


    [Required]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    //werknemer die de alert verwerkt
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public Ticket? Ticket { get; set; }


}
