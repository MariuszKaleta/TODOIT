using GraphQL.Types;
using TODOIT.GraphQl.Types.User;
using TODOIT.Model.Entity.Chat;

namespace TODOIT.GraphQl.Types.Chat
{
    public class MessageGqlType : ObjectGraphType<Message>
    {
        public MessageGqlType()
        {
            Field(o => o.Author,type: typeof(UserGqlType));
            Field(o => o.Chat, type: typeof(ChatGqlType));
            Field(o => o.CreateTime);
            Field(o => o.Text);
        }
    }
}