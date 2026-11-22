using Soc.Data.Entities.Enums;

namespace Soc.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class Contract
{
    [Key]
    public int ContractId { get; set; }
    [Required]
    public int SlaInDays { get; set; }

    [Required]
    public ConfidentialityLevel Confidentiality { get; set; }

    [Required]
    public DateTimeOffset StartDate { get; set; }
    [Required]
    public DateTimeOffset EndDate { get; set; }

    public int CustomerOrganizationId { get; set; }
    public CustomerOrganization CustomerOrganization { get; set; } = null!;

    // temporary tot we een fix voor opslag hebben
    public string ContractContent { get; set; }

}
