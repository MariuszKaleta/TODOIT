using GraphQL.Types;
using TODOIT.GraphQl.Types.Order;
using TODOIT.GraphQl.Types.User;

namespace TODOIT.GraphQl.Types.Opinion
{
    public class OpinionGQLType : ObjectGraphType<Model.Entity.Rate.Opinion>
    {
        public OpinionGQLType()
        {
            Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property from the owner object.");
            Field(x => x.Comment, type: typeof(StringGraphType)).Description("Id property from the owner object.");
            Field(x => x.Quality, type: typeof(RateGQLType)).Description("Id property from the owner object.");
            Field(x => x.Salary, type: typeof(RateGQLType)).Description("Id property from the owner object.");
            Field(x => x.Order, type: typeof(OrderGQLType)).Description("Id property from the owner object.");
            Field(x => x.Author, type: typeof(UserGqlType)).Description("Id property from the owner object.");
        }
    }
}
