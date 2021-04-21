using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using TODOIT.ViewModel.Opinion;
using TODOIT.ViewModel.Order;

namespace TODOIT.GraphQl.Types.Opinion
{
    public class OpinionInput : InputObjectGraphType<CreateOpinionViewModel>
    {
        public OpinionInput()
        {
            Name = nameof(Skill);

            Field(x => x.OrderId, nullable: false);
            Field(x => x.Quality, type: typeof(RateGQLType), nullable:true);
            Field(x => x.Salary, type: typeof(RateGQLType), nullable: true);
            Field(x => x.Comment, nullable: true);
        }
    }
}
