using System;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.Controllers.Api;
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
            .Argument<InputOwnerGraphType>("owner")
            .Resolve(CreateOwner);
        Field<OwnerGraphType>("updateOwner")
            .Argument<NonNullGraphType<IntGraphType>>("id")
            .Argument<InputOwnerGraphType>("owner")
            .Resolve(UpdateOwner);
        Field<OwnerGraphType>("deleteOwner")
            .Argument<IntGraphType>("id")
            .Resolve(DeleteOwner);
    }

    private Owner CreateOwner(IResolveFieldContext<object> context)
    {
        var owner = context.GetArgument<Owner>("owner");
        var id = _db.CreateOwner(owner);
        return _db.FindOwner(id);
    }

    
    
    private Owner UpdateOwner(IResolveFieldContext<object> context)
    {
        var updOwner = context.GetArgument<Owner>("owner");
        var id = context.GetArgument<int>("id").ToString();
        var owner = _db.FindOwner(id);
        owner.Name = updOwner.Name;
        owner.SecondName = updOwner.SecondName;
        owner.Mail = updOwner.Mail;
        owner.PhoneNumber = updOwner.PhoneNumber;
        owner.VehicleRegistration = updOwner.VehicleRegistration;
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