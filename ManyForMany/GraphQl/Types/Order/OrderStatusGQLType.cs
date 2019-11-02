using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using TODOIT.Model.Entity.Order;

namespace TODOIT.GraphQl.Types.Order
{
    public class OrderStatusGQLType : EnumerationGraphType<OrderStatus>
    {
    }
}
