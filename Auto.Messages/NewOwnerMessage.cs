﻿namespace Auto.Messages;

public class NewOwnerMessage
{
    public string OwnerId { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string Mail { get; set; }
    public string PhoneNumber { get; set; }
    
    public string VehicleRegistration { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
}

public class OwnerWithHashData : NewOwnerMessage
{
    public string Hash { get; private set; }

    public OwnerWithHashData(NewOwnerMessage owner,string hash)
    {
        OwnerId = owner.OwnerId;
        Hash = hash;
        FirstName = owner.FirstName;
        SecondName = owner.SecondName;
        PhoneNumber = owner.PhoneNumber;
        Mail = owner.Mail;
        VehicleRegistration = owner.VehicleRegistration;
    }
    
    
}