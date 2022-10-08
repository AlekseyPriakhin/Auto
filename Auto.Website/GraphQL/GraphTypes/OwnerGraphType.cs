using Auto.Data.Entities;
using GraphQL.Types;

namespace Auto.Website.GraphQL.GraphTypes;

public class OwnerGraphType : ObjectGraphType<Owner>
{
    public OwnerGraphType()
    {
        Name = "owner";
        Field(o => o.Name);
        Field(o => o.SecondName);
        Field(o => o.Mail);
        Field(o => o.OwnerId);
        Field(o => o.PhoneNumber);
        Field(o => o.Vehicle, nullable:false, typeof(VehicleGraphType));
        
    }
}