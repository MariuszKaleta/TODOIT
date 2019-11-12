using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GraphQL.Language.AST;
using GraphQL.Tests.Subscription;
using GraphQL.Types;
using GraphQlHelper;
using TODOIT.GraphQl.Types.Chat;
using TODOIT.GraphQl.Types.Opinion;
using TODOIT.GraphQl.Types.Order;
using TODOIT.GraphQl.Types.Skill;
using TODOIT.GraphQl.Types.User;
using TODOIT.Model.Entity.Chat;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.Rate;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.Order;

namespace TODOIT.GraphQl.Queries
{
    public class AppQuery : ObjectGraphType
    {
        public AppQuery(IOrderRepository orderRepository, IUserRepository userRepository, ISkillRepository skillRepository, IOpinionRepository opinionRepository, IChatRepository chatRepository, IMessageRepository messageRepository)
        {
            orderRepository.BaseQuerry<OrderGQLType, Order, Guid>(nameof(Order), this, fields =>
            {
                var list = new List<Expression<Func<Order, object>>>();

                if (fields.ContainsKey(nameof(Order.Owner).ToLower()))
                {
                    list.Add(x => x.Owner);
                }

                return list.ToArray();
            });

            userRepository.BaseQuerry<UserGqlType, ApplicationUser, string>("User", this, fields =>
                {
                    return Enumerable.Empty<Expression<Func<ApplicationUser, object>>>().ToArray();
                });

            Skills(skillRepository, nameof(Skill), this, fields =>
            {
                return Enumerable.Empty<Expression<Func<Skill, object>>>().ToArray();
            });

            Opinions(opinionRepository, nameof(Opinion), this, x =>
            {
                var list = new List<Expression<Func<Opinion, object>>>();

                if (x.ContainsKey(nameof(Opinion.Order).ToLower()))
                {
                    list.Add(x => x.Order);
                }

                if (x.ContainsKey(nameof(Opinion.Author).ToLower()))
                {
                    list.Add(x => x.Author);
                }

                return list.ToArray();

            });

            Chats(chatRepository, this, fields =>
            {
                var list = new List<Expression<Func<Chat, object>>>();

                if (fields.ContainsKey(nameof(Chat.Order).ToLower()))
                {
                    list.Add(x => x.Order);
                }

                return list.ToArray();
            });

            Messages(messageRepository, this, fields =>
            {
                var list = new List<Expression<Func<Message, object>>>();

                if (fields.ContainsKey(nameof(Message.Author).ToLower()))
                {
                    list.Add(x => x.Author);
                }

                if (fields.ContainsKey(nameof(Message.Chat).ToLower()))
                {
                    list.Add(x => x.Chat);
                }

                return list.ToArray();
            });
        }
        
        public static void Skills(ISkillRepository repository, string nameType, ObjectGraphType obj, Func<IDictionary<string, Field>, Expression<Func<Skill, object>>[]> include)
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

                    return repository.Get( name, star, coun, include.Invoke(context.SubFields));
                });

            obj.Field<SkillGqlType>(
                nameType,
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = nameof(IBaseElement.Name) }),
                resolve: context =>
                {
                    var id = context.GetArgument<string>(nameof(IBaseElement.Name).ToLower());

                    return repository.Get(id, include.Invoke(context.SubFields));
                });
        }
        
        public static void Opinions(IOpinionRepository repository, string nameType, ObjectGraphType obj, Func<IDictionary<string, Field>, Expression<Func<Opinion, object>>[]> include)
        {
            const string start = "start";
            const string count = "count";

            obj.Field<ListGraphType<OpinionGQLType>>(
                nameType + "s",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> {Name = start},
                    new QueryArgument<IntGraphType> {Name = count}
                ),
                resolve: context =>
                {
                    var star = context.GetArgument<int?>(start);
                    var coun = context.GetArgument<int?>(count);

                    return repository.Get(star, coun, include.Invoke(context.SubFields));
                });

            obj.Field<OpinionGQLType>(
                nameType,
                arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = nameof(Opinion.Id) }),
                resolve: context =>
                {
                    var id = context.GetArgument<Guid>(nameof(Opinion.Id).ToLower());

                    return repository.Get(id, include.Invoke(context.SubFields));
                });
        }

        public static void Chats(IChatRepository repository,  ObjectGraphType obj, Func<IDictionary<string, Field>, Expression<Func<Chat, object>>[]> include)
        {
            const string start = "start";
            const string count = "count";

            obj.Field<ListGraphType<ChatGqlType>>(
                 nameof(Chat) + "s",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = nameof(Order.Name) },
                    new QueryArgument<IntGraphType> { Name = start },
                    new QueryArgument<IntGraphType> { Name = count }
                    ),
                resolve: context =>
                {
                    var star = context.GetArgument<int?>(start);
                    var coun = context.GetArgument<int?>(count);
                    var name = context.GetArgument<string>(nameof(Order.Name).ToLower());

                    return repository.Get(name,star,coun, include.Invoke(context.SubFields));
                });

            obj.Field<ChatGqlType>(
                nameof(Chat),
                arguments: new QueryArguments(new QueryArgument<IdGraphType> { Name = nameof(Opinion.Id) }),
                resolve: context =>
                {
                    var id = context.GetArgument<Guid>(nameof(Opinion.Id).ToLower());

                    return repository.Get(id, include.Invoke(context.SubFields));
                });
        }

        public static void Messages(IMessageRepository repository, ObjectGraphType obj, Func<IDictionary<string, Field>, Expression<Func<Message, object>>[]> include)
        {
            const string start = "start";
            const string count = "count";

            obj.Field<ListGraphType<MessageGqlType>>(
                nameof(Message) + "s",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = nameof(Message.Id) },
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = nameof(Message.Text) },
                    new QueryArgument<IntGraphType> { Name = start },
                    new QueryArgument<IntGraphType> { Name = count }
                ),
                resolve: context =>
                {
                    var star = context.GetArgument<int?>(start);
                    var coun = context.GetArgument<int?>(count);
                    var name = context.GetArgument<string>(nameof(Message.Text).ToLower());
                    var chatId = context.GetArgument<Guid>(nameof(Chat.Id).ToLower());

                    return repository.Get(chatId, name, star, coun, include.Invoke(context.SubFields));
                });
        }
      

    }
}
