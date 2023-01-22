using System;
using System.Collections.Generic;
using Auto.Data;
using Auto.Website.GraphQL.Mutations;
using Auto.Website.GraphQL.Queries;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Auto.Website.GraphQL.Schemas;

public class AutoSchema : Schema
{
    public AutoSchema(IAutoDatabase db)
    {
        Query = new OwnerQuery(db);
        Mutation = new OwnerMutation(db);
    }
}