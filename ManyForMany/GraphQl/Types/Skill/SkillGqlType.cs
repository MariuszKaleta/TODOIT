using GraphQL.Types;

namespace TODOIT.GraphQl.Types.Skill
{
    public class SkillGqlType : ObjectGraphType<Model.Entity.Skill.Skill>
    {
        public SkillGqlType( )
        {
            Field(x => x.Name, type: typeof(StringGraphType)).Description("Id property from the owner object.");
        }
    }

}
