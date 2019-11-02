using System;
using GraphQL.Types;
using GraphQlHelper;
using TODOIT.GraphQl.Types.Order;
using TODOIT.Model.Entity.Order;
using TODOIT.Model.Entity.User;
using TODOIT.Repositories.Contracts;
using TODOIT.ViewModel.Order;

namespace TODOIT.GraphQl.Queries
{
    public class AppMutation : ObjectGraphType
    {
        public AppMutation(IOrderRepository orderRepository, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            //Orders(orderRepository, nameof(Order), this, userManager);
        }

        /*
        public static void Orders(IOrderRepository repository, string name, ObjectGraphType appMutation, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            appMutation.Field<OrderGQLType>(
                $"Create{name}",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<OrderInput>>()
                { Name = name })
                ,
                resolve: context =>
                {
                    var producer = context.GetArgument<CreateOrderViewModel>(name.ToLower());
                    return repository.Create(producer, null);
                }
            );

            var idName = name + nameof(IBaseElement.Id);

            appMutation.Field<OrderGQLType>(
                $"Update{name}",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<OrderInput>>() { Name = name },
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = idName }
                )
                ,
                resolve: context =>
                {
                    var producerId = context.GetArgument<Guid>(idName);
                    var producer = context.GetArgument<CreateOrderViewModel>(name.ToLower());

                    var dbProducer = repository.Get(producerId).Result;

                    return repository.Update(producer, dbProducer, null);
                }
            );

            appMutation.Field<OrderGQLType>(
                $"Delete{name}",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = idName }
                )
                ,
                resolve: context =>
                {
                    var producerId = context.GetArgument<Guid>(idName);

                    var dbProducer = repository.Get(producerId).Result;

                    repository.Delete(dbProducer, true, null);

                    return $"The element with the id: {producerId} has been successfully deleted from db.";
                }
            );
        }
*/

    }
}
