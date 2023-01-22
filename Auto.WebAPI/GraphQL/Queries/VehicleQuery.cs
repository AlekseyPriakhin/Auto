using System.Collections.Generic;
using System.Linq;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.MicrosoftDI;
using GraphQL.Types;

namespace Auto.Website.GraphQL.Queries;

public sealed class VehicleQuery : ObjectGraphType
{
    private readonly IAutoDatabase _db;

    public VehicleQuery(IAutoDatabase db)
    {
        _db = db;
        Field<VehicleGraphType>("vehicle")
            .Argument<string>("reg")
            .Resolve(GetVehicle);

        Field<ListGraphType<VehicleGraphType>>("vehicles")
            .Argument<int>("count")
            .Resolve(Vehicles);
    }

    private Vehicle GetVehicle(IResolveFieldContext<object> context)
    {
        return _db.FindVehicle(context.GetArgument<string>("reg"));
    }
    
    private IEnumerable<Vehicle> Vehicles(IResolveFieldContext<object> context)
    {
        var count = context.GetArgument<int>("count");
        if (count == default) count = 10;
        return _db.ListVehicles().Take(count);
    }

}