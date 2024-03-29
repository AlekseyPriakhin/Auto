﻿using Newtonsoft.Json;

namespace Auto.Data.Entities;

public class Owner
{
    public string OwnerId { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string Mail { get; set; }
    public string PhoneNumber { get; set; }
    
    public string VehicleRegistration { get; set; }
    
    [JsonIgnore]
    public virtual Vehicle Vehicle { get; set; }
}