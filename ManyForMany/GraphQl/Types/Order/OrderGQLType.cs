using System;
using GraphQL.DataLoader;
using GraphQL.Types;
using TODOIT.GraphQl.Types.Opinion;
using TODOIT.GraphQl.Types.Skill;
using TODOIT.GraphQl.Types.User;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;

namespace TODOIT.GraphQl.Types.Order
{
    public class OrderGQLType : ObjectGraphType<Model.Entity.Order.Order>
    {
        public OrderGQLType(IDataLoaderContextAccessor dataLoader, ISkillRepository skillRepository, IOpinionRepository opinionRepository, IUserRepository userRepository)
        {
            Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property from the owner object.");
            Field(x => x.Name, type: typeof(StringGraphType)).Description("Name property from the owner object.");
            Field(x => x.DeadLine, type: typeof(DateTimeGraphType)).Description("Description property from the owner object.");
            Field(x => x.CreateTime, type: typeof(DateTimeGraphType)).Description("Description property from the owner object.");
            Field(x => x.Describe, type: typeof(StringGraphType)).Description("Description property from the owner object.");
            Field(x => x.Owner, type: typeof(UserGqlType)).Description("Description property from the owner object.");
            Field(x => x.Status, type: typeof(OrderStatusGQLType)).Description("Description property from the owner object.");
            
            Field<ListGraphType<SkillGqlType>>(
                nameof(ReuiredSkill) + "s",
                resolve: context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<Guid, Model.Entity.Skill.Skill>(
                        nameof(skillRepository.GetByOrderIds), skillRepository.GetByOrderIds);

                    return loader.LoadAsync(context.Source.Id);
                });
            
            Field<ListGraphType<OpinionGQLType>>(
                nameof(Opinion) + "s",
                resolve: context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<Guid, Model.Entity.Rate.Opinion>(
                        nameof(opinionRepository.GetByOrderIds), opinionRepository.GetByOrderIds);

                    return loader.LoadAsync(context.Source.Id);
                });
            
            Field<ListGraphType<UserGqlType>>(
                "InterestedUsers",
                resolve: context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<Guid, ApplicationUser>(
                        nameof(userRepository.GetInterestedByOrderIds), userRepository.GetInterestedByOrderIds);

                    return loader.LoadAsync(context.Source.Id);
                });
                
        }
    }
}
