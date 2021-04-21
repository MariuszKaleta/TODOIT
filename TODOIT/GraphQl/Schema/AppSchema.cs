using System;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using TODOIT.GraphQl.Queries;
using TODOIT.Model.Entity.Chat;
using TODOIT.Repositories.Contracts;

namespace TODOIT.GraphQl.Schema
{

    public class AppSchema : GraphQL.Types.Schema
    {
        public AppSchema(IServiceProvider resolver) : base(resolver)
        {
            Query = new AppQuery(
                resolver.GetService<IOrderRepository>(),
                resolver.GetService<IUserRepository>(),
                resolver.GetService<ISkillRepository>(),
                resolver.GetService<IOpinionRepository>(),
                resolver.GetService<IChatRepository>(),
                resolver.GetService<IMessageRepository>()
                );

            Mutation = resolver.GetService<AppMutation>();
        }
    }
}
