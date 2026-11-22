namespace Soc.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class Organization
{
    [Key]
    public int OrgId { get; set; }
    [Required]
    public string Name { get; set; }

    public ICollection<CustomerOrganization> CustomerOrganizations { get; set; } = new List<CustomerOrganization>();

}
