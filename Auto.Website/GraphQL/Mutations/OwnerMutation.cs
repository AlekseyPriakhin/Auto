using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Mutations;

public class OwnerMutation : ObjectGraphType
{
    private readonly IAutoDatabase _db;

    public OwnerMutation(IAutoDatabase db)
    {
        _db = db;

        Field<OwnerGraphType>("createOwner")
            .Argument<string>("name")
            .Resolve(CreateOwner);
        Field<OwnerGraphType>("updateOwner")
            .Argument<int>("id")
            .Argument<string>("newName")
            .Argument<string>("newSecondName")
            .Resolve(UpdateOwner);
        Field<OwnerGraphType>("deleteOwner")
            .Argument<int>("id")
            .Resolve(DeleteOwner);
    }

    private Owner CreateOwner(IResolveFieldContext<object> context)
    {
        var name = context.GetArgument<string>("name");

        var newOwner = new Owner
        {
            Name = name,
            SecondName = "secondName",
            Mail = "email",
            PhoneNumber = "1111",
            VehicleRegistration = "HVT266H"
        };

        var id = _db.CreateOwner(newOwner);

        return _db.FindOwner(id);
    }

    private Owner UpdateOwner(IResolveFieldContext<object> context)
    {
        var id = context.GetArgument<int>("id").ToString();
        var owner = _db.FindOwner(id);
        owner.Name = context.GetArgument<string>("newName");
        owner.SecondName = context.GetArgument<string>("newSecondName");
        _db.UpdateOwner(owner);
        return _db.FindOwner(id);
    }

    private Owner DeleteOwner(IResolveFieldContext<object> context)
    {
        var id = context.GetArgument<int>("id").ToString();
        var owner = _db.FindOwner(id);
        _db.DeleteOwner(id);
        return owner;
    }
    
    
}