using Auto.Data.Entities;
using GraphQL.Types;

namespace Auto.Website.GraphQL.GraphTypes;

public class InputOwnerGraphType : InputObjectGraphType<Owner>
{
    public InputOwnerGraphType()
    {
        Name = "InputOwnerType";
        Field<NonNullGraphType<StringGraphType>>("name");
        Field<NonNullGraphType<StringGraphType>>("secondName");
        Field<NonNullGraphType<StringGraphType>>("mail");
        Field<NonNullGraphType<StringGraphType>>("phoneNumber");
        Field<NonNullGraphType<StringGraphType>>("vehicleRegistration");
    }
}