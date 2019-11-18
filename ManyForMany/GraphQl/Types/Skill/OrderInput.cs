using GraphQL.Types;
using TODOIT.ViewModel.Order;
using TODOIT.ViewModel.Skill;

namespace TODOIT.GraphQl.Types.Order
{
    public class SkillInput : InputObjectGraphType<CreateSkillViewModel>
    {
        public SkillInput()
        {
            Name = nameof(SkillInput);

            Field(x => x.Name, type: typeof(NonNullGraphType<StringGraphType>));

            //Field(x => x.Categories, type: typeof(ListGraphType<IntGraphType>));
            //Field(x => x.GoodIfHave, type: typeof(ListGraphType<IntGraphType>));
            //Field(x => x.RequiredSkills, type: typeof(ListGraphType<IntGraphType>));

        }
    }
}
