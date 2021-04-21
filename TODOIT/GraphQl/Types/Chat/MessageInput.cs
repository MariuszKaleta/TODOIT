using GraphQL.Types;

namespace TODOIT.GraphQl.Types.Chat
{
    public class MessageInput : InputObjectGraphType<string>
    {
        public MessageInput()
        {
            Name = nameof(MessageInput);

            Field(x => x);
        }
    }
}
