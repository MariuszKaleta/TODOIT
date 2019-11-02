using GraphQL.Types;
using TODOIT.ViewModel.Order;

namespace TODOIT.GraphQl.Types.Order
{
    public class OrderInput : InputObjectGraphType<CreateOrderViewModel>
    {
        public OrderInput()
        {
            Name = nameof(OrderInput);

            Field(x => x.Name, type: typeof(NonNullGraphType<StringGraphType>));
            Field(x => x.DeadLine, type: typeof(DateTimeGraphType));
            Field(x => x.Describe, type: typeof(StringGraphType));

            //Field(x => x.Categories, type: typeof(ListGraphType<IntGraphType>));
            //Field(x => x.GoodIfHave, type: typeof(ListGraphType<IntGraphType>));
            //Field(x => x.RequiredSkills, type: typeof(ListGraphType<IntGraphType>));

        }
    }
}
