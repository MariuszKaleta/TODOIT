using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using TODOIT.ViewModel.Opinion;
using TODOIT.ViewModel.Order;

namespace TODOIT.GraphQl.Types.Opinion
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
