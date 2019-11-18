using System;
using GraphQL.Tests.Subscription;
using GraphQL.Types;
using GraphQlHelper;
using TODOIT.GraphQl.Types.Order;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.Order;
using TODOIT.GraphQl;
using TODOIT.GraphQl.Types.Opinion;
using TODOIT.GraphQl.Types.Skill;
using TODOIT.Model.Configuration;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.Skill;
using TODOIT.ViewModel.Opinion;
using TODOIT.ViewModel.Skill;
using TODOIT.ViewModel.User;

namespace TODOIT.GraphQl.Queries
{
    public class AppMutation : ObjectGraphType
    {
        public AppMutation(IOrderRepository orderRepository, IUserRepository userRepository, 
            IChatRepository chatRepository, IOrderMembersRepository orderMembersRepository, 
            IOpinionRepository opinionRepository, ISkillRepository skillRepository,
            IMessageRepository messageRepository)
        {
            Orders(orderRepository, chatRepository, nameof(Order), this);

            OrderMembers(orderMembersRepository, orderRepository, chatRepository, "user", this);

            Opinions(opinionRepository, nameof(Opinion), this);

            Skills(skillRepository, nameof(Skill), this);

            Users(userRepository, "ser", this);

            Message(messageRepository, this);
        }


        public static void OrderMembers(IOrderMembersRepository repository, IOrderRepository orderRepository, IChatRepository chatRepository, string name, ObjectGraphType appMutation)
        {
            var idName = name + nameof(IBaseElement.Id);
            var idOrder = nameof(Order) + nameof(IBaseElement.Id);

            appMutation.Field<OrderGQLType>()
                .Name($"Add{name}ToTeam")
                .Argument<ListGraphType<IdGraphType>>(idName)
                .Argument<NonNullGraphType<IdGraphType>>(idOrder)
                .ResolveAsync(async context =>
                {
                    var userId = context.GetArgument<string[]>(idName);
                    var orderId = context.GetArgument<Guid>(idOrder);
                    var ownerId = context.UserContext.GetUserId();
                    await repository.InviteUserToMakeOrder(orderRepository, chatRepository, orderId, ownerId, userId);

                    return $"The element with the id: {orderId} has been successfully removed from interested";
                });

            appMutation.Field<OrderGQLType>()
                .Name($"Kick{name}UserFromTeam")
                .Argument<ListGraphType<IdGraphType>>(idName)
                .Argument<NonNullGraphType<IdGraphType>>(idOrder)
                .ResolveAsync(async context =>
                {
                    var userId = context.GetArgument<string[]>(idName);
                    var orderId = context.GetArgument<Guid>(idOrder);
                    var ownerId = context.UserContext.GetUserId();
                    await repository.KickUserFromMakeOrder(orderRepository, chatRepository, orderId, ownerId, userId);

                    return $"The element with the id: {orderId} has been successfully removed from interested";
                });
        }

        public static void Orders(IOrderRepository repository, IChatRepository chatRepository, string name, ObjectGraphType appMutation)
        {
            appMutation
                .Field<OrderGQLType>()
                .Name($"Create{name}")
                .Argument<NonNullGraphType<OrderInput>>(name)
                .ResolveAsync(async context =>
                {
                    var user = context.UserContext.GetUserId();

                    var producer = context.GetArgument<CreateOrderViewModel>(name.ToLower());

                    var order = await repository.Create(producer, user);
                    await chatRepository.Create(order.Id);

                    return order;
                });

            var idName = name + nameof(IBaseElement.Id);

            appMutation.Field<OrderGQLType>()
                .Name($"Update{name}")
                .Argument<NonNullGraphType<OrderInput>>(name)
                .Argument<NonNullGraphType<IdGraphType>>(idName)
                .ResolveAsync(async context =>
                    {
                        var userId = context.UserContext.GetUserId();
                        var orderId = context.GetArgument<Guid>(idName);
                        var model = context.GetArgument<CreateOrderViewModel>(name.ToLower());

                        return repository.Update(model, orderId, userId);
                    }
                );

            appMutation.Field<OrderGQLType>()
                .Name($"Delete{name}")
                .Argument<NonNullGraphType<IdGraphType>>(idName)
                .ResolveAsync(async context =>
                {
                    var orderId = context.GetArgument<Guid>(idName);
                    var userId = context.UserContext.GetUserId();
                    repository.Delete(orderId, userId);

                    return $"The element with the id: {orderId} has been successfully deleted from db.";
                });

            appMutation.Field<OrderGQLType>()
                .Name($"Add{name}ToInterested")
                .Argument<NonNullGraphType<IdGraphType>>(idName)
                .ResolveAsync(async context =>
                {
                    var orderId = context.GetArgument<Guid>(idName);
                    var userId = context.UserContext.GetUserId();
                    await repository.AddToInterested(userId, orderId);

                    return $"The element with the id: {orderId} has been successfully added to interested";
                });

            appMutation.Field<OrderGQLType>()
                .Name($"Remove{name}FromInterested")
                .Argument<NonNullGraphType<IdGraphType>>(idName)
                .ResolveAsync(async context =>
                {
                    var orderId = context.GetArgument<Guid>(idName);
                    var userId = context.UserContext.GetUserId();
                    await repository.RemoveFromInterested(userId, orderId);

                    return $"The element with the id: {orderId} has been successfully removed from interested";
                });

        }

        public static void Opinions(IOpinionRepository repository, string name, ObjectGraphType appMutation)
        {
            appMutation
                .Field<OpinionGQLType>()
                .Name($"Create{name}")
                .Argument<NonNullGraphType<OpinionInput>>(name)
                .ResolveAsync(async context =>
                {
                    var user = context.UserContext.GetUserId();

                    var model = context.GetArgument<CreateOpinionViewModel>(name.ToLower());

                    return await repository.Create(model, user);
                });

            var idName = name + nameof(IBaseElement.Id);

            appMutation.Field<OpinionGQLType>()
                .Name($"Update{name}")
                .Argument<NonNullGraphType<OpinionInput>>(name)
                .Argument<NonNullGraphType<IdGraphType>>(idName)
                .ResolveAsync(async context =>
                {
                    var userId = context.UserContext.GetUserId();
                    var opinionId = context.GetArgument<Guid>(idName);
                    var model = context.GetArgument<CreateOpinionViewModel>(name.ToLower());

                    return repository.Update(opinionId, model, userId);
                });

            appMutation.Field<OpinionGQLType>()
                .Name($"Delete{name}")
                .Argument<NonNullGraphType<IdGraphType>>(idName)
                .ResolveAsync(async context =>
                {
                    var orderId = context.GetArgument<Guid>(idName);
                    var userId = context.UserContext.GetUserId();
                    await repository.Delete(orderId, userId);

                    return $"The element with the id: {orderId} has been successfully deleted from db.";
                });
        }
        
        public static void Skills(ISkillRepository repository, string name, ObjectGraphType appMutation)
        {
            appMutation
                .Field<SkillGqlType>()
                .Name($"Create{name}")
                .Argument<NonNullGraphType<SkillInput>>(name)
                .ResolveAsync(async context =>
                {
                    var model = context.GetArgument<CreateSkillViewModel>(name.ToLower());

                    return await repository.Create(model);
                });

            var idName = name + nameof(IBaseElement.Id);

            appMutation.Field<SkillGqlType>()
                .Name($"Update{name}")
                .Argument<NonNullGraphType<SkillInput>>(name)
                .Argument<NonNullGraphType<IdGraphType>>(idName)
                .ResolveAsync(async context =>
                {
                    var skillName = context.GetArgument<string>(idName);
                    var model = context.GetArgument<CreateSkillViewModel>(name.ToLower());

                    return repository.Update(skillName, model);
                });

            appMutation.Field<SkillGqlType>()
                .Name($"Delete{name}")
                .Argument<NonNullGraphType<IdGraphType>>(idName)
                .ResolveAsync(async context =>
                {
                    var orderId = context.GetArgument<string>(idName);

                    repository.Delete(orderId, true);

                    return $"The element with the id: {orderId} has been successfully deleted from db.";
                });
        }

        public static void Users(IUserRepository repository, string name, ObjectGraphType appMutation)
        {
            var idName = name + nameof(IBaseElement.Id);

            appMutation.Field<SkillGqlType>()
                .Name($"Update{name}")
                .Argument<NonNullGraphType<UserInput>>(name)
                .ResolveAsync(async context =>
                {
                    var user = context.UserContext.GetUserId();

                    var model = context.GetArgument<UserViewModel>(name.ToLower());

                    return repository.Update(user, model);
                });
        }
        
        public static void Message(IMessageRepository repository, ObjectGraphType appMutation)
        {
            var idName = "chatId";
            var name = "message";

            appMutation.Field<MessageGqlType>()
                .Name($"sendMessage")
                .Argument<NonNullGraphType<MessageInput>>(name)
                .Argument<NonNullGraphType<IdGraphType>>(idName)
                .ResolveAsync(async context =>
                {
                    var user = context.UserContext.GetUserId();

                    var model = context.GetArgument<string>(name.ToLower());
                    var id = context.GetArgument<Guid>(idName.ToLower());

                    return repository.Create(user, id, model, true);
                });
        }

    }
}
