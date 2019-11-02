using System;
using GraphQL.Types;
using GraphQlHelper;
using TODOIT.GraphQl.Types.Order;
using TODOIT.GraphQl.Types.Skill;
using TODOIT.GraphQl.Types.User;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;

namespace TODOIT.GraphQl.Queries
{
    public class AppQuery : ObjectGraphType
    {
        public AppQuery(IOrderRepository orderRepository, IUserRepository userRepository, ISkillRepository skillRepository)
        {
            orderRepository.BaseQuerry<OrderGQLType, Order, Guid>(nameof(Order), this);
            userRepository.BaseQuerry<UserGqlType, ApplicationUser, string>("User", this);
            Skills(skillRepository, nameof(Skill), this);
        }

        public static void Skills(ISkillRepository repository, string nameType, ObjectGraphType obj)
        {
            const string start = "start";
            const string count = "count";

            obj.Field<ListGraphType<SkillGqlType>>(
                nameType + "s",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = nameof(IBaseElement.Name) },
                    new QueryArgument<IntGraphType> { Name = start },
                    new QueryArgument<IntGraphType> { Name = count }
                ),
                resolve: context =>
                {
                    var name = context.GetArgument<string>(nameof(IBaseElement.Name).ToLower());
                    var star = context.GetArgument<int?>(start);
                    var coun = context.GetArgument<int?>(count);

                    return repository.Get(name, star, coun);
                });

            obj.Field<SkillGqlType>(
                nameType,
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = nameof(IBaseElement.Name) }),
                resolve: context =>
                {
                    var id = context.GetArgument<string>(nameof(IBaseElement.Name).ToLower());

                    return repository.Get(id);
                });
        }
    }
}
