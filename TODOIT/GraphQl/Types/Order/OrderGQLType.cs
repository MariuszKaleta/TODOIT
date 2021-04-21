using System;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using GraphQL.Types;
using TODOIT.GraphQl.Types.Chat;
using TODOIT.GraphQl.Types.Opinion;
using TODOIT.GraphQl.Types.Skill;
using TODOIT.GraphQl.Types.User;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;

namespace TODOIT.GraphQl.Types.Order
{
    public class OrderGQLType : ObjectGraphType<Model.Entity.Order.Order>
    {
        public OrderGQLType(IDataLoaderContextAccessor dataLoader, ISkillRepository skillRepository, IOpinionRepository opinionRepository, IUserRepository userRepository, IChatRepository chatRepository)
        {
            Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property from the owner object.");
            Field(x => x.Name, type: typeof(StringGraphType)).Description("Name property from the owner object.");
            Field(x => x.DeadLine, type: typeof(DateTimeGraphType)).Description("Description property from the owner object.");
            Field(x => x.CreateTime, type: typeof(DateTimeGraphType)).Description("Description property from the owner object.");
            Field(x => x.Describe, type: typeof(StringGraphType)).Description("Description property from the owner object.");
            Field(x => x.Owner, type: typeof(UserGqlType)).Description("Description property from the owner object.");
            Field(x => x.Status, type: typeof(OrderStatusGQLType)).Description("Description property from the owner object.");

            Field<ListGraphType<SkillGqlType>>()
                .Name(nameof(RequiredSkill) + "s")
                .ResolveAsync(async context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<Guid, Model.Entity.Skill.Skill>(
                        nameof(skillRepository.GetByOrderIds), skillRepository.GetByOrderIds);

                    return await loader.LoadAsync(context.Source.Id);
                });

            Field<ListGraphType<OpinionGQLType>>()
                .Name(nameof(Opinion) + "s")
                .ResolveAsync(async context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<Guid, Model.Entity.Rate.Opinion>(
                        nameof(opinionRepository.GetByOrderIds), opinionRepository.GetByOrderIds);

                    return await loader.LoadAsync(context.Source.Id);
                });
            
            Field<ListGraphType<UserGqlType>>()
                .Name( "InterestedUsers")
                .ResolveAsync(async context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<Guid, ApplicationUser>(
                        nameof(userRepository.GetInterestedByOrderIds), userRepository.GetInterestedByOrderIds);

                    return await loader.LoadAsync(context.Source.Id);
                });

            Field<ListGraphType<UserGqlType>>()
                .Name(nameof(OrderMember) + "s")
                .ResolveAsync(async context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<Guid, ApplicationUser>(
                        nameof(userRepository.GetOrderMembersByOrderIds), userRepository.GetOrderMembersByOrderIds);

                    return await loader.LoadAsync(context.Source.Id);
                });

            Field<ChatGqlType>()
                .Name(nameof(Chat))
                .ResolveAsync(async ContextBoundObject => { return chatRepository.GetByOrderId(ContextBoundObject.Source.Id); });
        }
    }
}
