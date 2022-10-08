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
        Field<OwnerGraphType>("owners")
            .Argument<int>("ownerId")
            .Resolve(GetOwner);
    }
    


    private Owner GetOwner(IResolveFieldContext<object> context)
    {
        var id = context.GetArgument<int>("ownerId").ToString();
        var a = _db.FindOwner(id);
        if (a == null) return null;
        return _db.FindOwner(id);
    }
}