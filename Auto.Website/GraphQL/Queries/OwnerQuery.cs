using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Queries;

public class OwnerQuery : ObjectGraphType
{
    private IAutoDatabase _db;

    public OwnerQuery(IAutoDatabase db)
    {
        _db = db;
        Field<OwnerGraphType>("ownerById")
            .Argument<IntGraphType>("id")
            .Resolve(OwnerById);
        Field<ListGraphType<OwnerGraphType>>("owners")
            .Resolve(GetOwners);
        Field<OwnerGraphType>("ownerByFullName")
            .Argument<StringGraphType>("name")
            .Resolve(OwnerByFullName);
        Field<OwnerGraphType>("ownerByVehicleRegistration")
            .Argument<StringGraphType>("registration")
            .Resolve(OwnerByVehicleRegistration);
    }



    private IEnumerable<Owner> GetOwners(IResolveFieldContext<object> context)
    {
        return _db.ListOwners();
    }
    
    private Owner OwnerById(IResolveFieldContext<object> context)
    {
        var id = context.GetArgument<int>("id").ToString();
        return _db.FindOwner(id);
    }

    private Owner OwnerByVehicleRegistration(IResolveFieldContext<object> context)
    {
        var registration = context.GetArgument<string>("registration");
        var owner = _db.FindVehicle(registration).Owner;
        return owner;
    }
    
    private Owner OwnerByFullName(IResolveFieldContext<object> context)
    {
        var tokens = context.GetArgument<string>("name").Split(" ");
        var owner = _db.ListOwners()
            .FirstOrDefault(
                o => o.Name.Equals(tokens[0]) 
                     && o.SecondName.Equals(tokens[1])
                     );
        return owner;
    }
}