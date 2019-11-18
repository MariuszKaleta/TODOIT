using GraphQL.Types;
using TODOIT.ViewModel.Order;
using TODOIT.ViewModel.User;

namespace TODOIT.GraphQl.Types.Order
{
    public class UserInput : InputObjectGraphType<UserViewModel>
    {
        public UserInput()
        {
            Name = nameof(UserInput);

            Field(x => x.Skills, nullable: true);

            //Field(x => x.Categories, type: typeof(ListGraphType<IntGraphType>));
            //Field(x => x.GoodIfHave, type: typeof(ListGraphType<IntGraphType>));
            //Field(x => x.RequiredSkills, type: typeof(ListGraphType<IntGraphType>));

        }
    }
}
