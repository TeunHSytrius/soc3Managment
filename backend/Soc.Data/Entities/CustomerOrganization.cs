namespace Soc.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class CustomerOrganization
{
    [Key]
    public int CustomerOrgId { get; set; }
    [Required]
    public int CustomerId { get; set; }
    public  Customer Customer { get; set; }

    [Required]
    public int organizationId { get; set; }
    public  Organization Organization { get; set; }

    public  Contract? Contract { get; set; }


}
