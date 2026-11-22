namespace Soc.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Email { get; set; }

    public ICollection<CustomerOrganization>  CustomerOrganizations { get; set; } = new List<CustomerOrganization>();
}
