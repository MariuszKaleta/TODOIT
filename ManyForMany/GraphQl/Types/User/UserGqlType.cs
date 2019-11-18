using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using GraphQL.Types;
using TODOIT.GraphQl.Types.Opinion;
using TODOIT.GraphQl.Types.Order;
using TODOIT.GraphQl.Types.Skill;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;

namespace TODOIT.GraphQl.Types.User
{
    public class UserGqlType : ObjectGraphType<ApplicationUser>
    {
        public UserGqlType(IDataLoaderContextAccessor dataLoader, IOrderRepository orderRepository, ISkillRepository skillRepository, IOpinionRepository opinionRepository)
        {
            Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property from the owner object.");
            Field(x => x.UserName, type: typeof(StringGraphType)).Description("Id property from the owner object.");
            Field(x => x.Name, type: typeof(StringGraphType)).Description("Id property from the owner object.");
            Field(x => x.Surrname, type: typeof(StringGraphType)).Description("Id property from the owner object.");
            Field(x => x.Email, type: typeof(StringGraphType)).Description("Id property from the owner object.");
            Field(x => x.PhoneNumber, type: typeof(StringGraphType)).Description("Id property from the owner object.");

            Field<ListGraphType<OrderGQLType>>()
                  .Name(nameof(Order) + "s")
                  .ResolveAsync(async context =>
                  {
                      var ordersLoader = dataLoader.Context.GetOrAddCollectionBatchLoader<string, Model.Entity.Order.Order>(
                        nameof(orderRepository.GetByOwnerIds),
                        orderRepository.GetByOwnerIds);

                      return await ordersLoader.LoadAsync(context.Source.Id);
                  });

            Field<ListGraphType<OrderGQLType>>()
                .Name(nameof(InterestedOrder) + "s")
                .ResolveAsync(async context =>
                {
                    var interestedOrdersLoader = dataLoader.Context.GetOrAddCollectionBatchLoader<string, Model.Entity.Order.Order>(
                        nameof(orderRepository.GetByInterestedOrderIds),
                        orderRepository.GetByInterestedOrderIds);

                    return await interestedOrdersLoader.LoadAsync(context.Source.Id);
                });

            Field<ListGraphType<SkillGqlType>>(
                nameof(HeadSkill) + "s",
                resolve: context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<string, Model.Entity.Skill.Skill>(
                        nameof(skillRepository.GetByUserIds), skillRepository.GetByUserIds);

                    return loader.LoadAsync(context.Source.Id);
                });

            Field<ListGraphType<OpinionGQLType>>(
                nameof(Opinion) + "s",
                resolve: context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<string, Model.Entity.Rate.Opinion>(
                        nameof(opinionRepository.GetByAuthorIds), opinionRepository.GetByAuthorIds);

                    return loader.LoadAsync(context.Source.Id);
                });
        }
    }
}
