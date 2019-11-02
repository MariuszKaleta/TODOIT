using System;
using Microsoft.Extensions.DependencyInjection;
using TODOIT.GraphQl.Queries;

namespace TODOIT.GraphQl.Schema
{

    public class AppSchema : GraphQL.Types.Schema
    {
        public AppSchema(IServiceProvider resolver) : base(resolver)
        {
            Query = resolver.GetService<AppQuery>();
            Mutation = resolver.GetService<AppMutation>();
        }
    }
}
