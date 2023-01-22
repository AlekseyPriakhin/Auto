using System.ComponentModel.DataAnnotations;

namespace WebApplication1;

public class OwnerViewModel
{
    public string Name { get; set; }
    public string SecondName { get; set; }
    [EmailAddress]
    public string Mail { get; set; }
    public string PhoneNumber { get; set; }
    
    public string VehicleRegistration { get; set; }
}