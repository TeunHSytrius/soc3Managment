namespace Soc.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class Evidence
{
    [Key]
    public int EvidenceId { get; set; }
    [Required]
    public int AnalystId { get; set; }
    public Analyst Analyst { get; set; }

    [Required]
    public int ResearchId { get; set; }
    public  Research Research { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    public string EvidenceType { get; set; }

    //Evidence content is temporary tot we een passende oplossing vinden
    [Required]
    public string EvidenceContent { get; set; }

}
