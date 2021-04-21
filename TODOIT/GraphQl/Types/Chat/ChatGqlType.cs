using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using GraphQL.Types;
using TODOIT.GraphQl.Types.Order;
using TODOIT.GraphQl.Types.Skill;
using TODOIT.GraphQl.Types.User;
using TODOIT.Model.Entity.Chat;
using TODOIT.Model.Entity.Skill;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;

namespace TODOIT.GraphQl.Types.Chat
{
    public class ChatGqlType : ObjectGraphType<Model.Entity.Chat.Chat>
    {
        public ChatGqlType(IDataLoaderContextAccessor dataLoader, IMessageRepository messageRepository, IChatRepository chatRepository)
        {
            Field(x => x.Order, type: typeof(OrderGQLType));
            Field(x => x.Id, nullable: false);

            Field<ListGraphType<MessageGqlType>>()
                .Name(nameof(Message) + "s")
                .ResolveAsync(async context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<Guid, Message>(
                        nameof(messageRepository.GetMessagesByChatIds), messageRepository.GetMessagesByChatIds);

                    return await loader.LoadAsync(context.Source.Id);
                });

            Field<ListGraphType<UserGqlType>>()
                .Name(nameof(ChatMember) + "s")
                .ResolveAsync(async context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<Guid, ApplicationUser>(
                        nameof(chatRepository.GetByChatIdMembers), chatRepository.GetByChatIdMembers);

                    return await loader.LoadAsync(context.Source.Id);
                });



        }
    }
}
