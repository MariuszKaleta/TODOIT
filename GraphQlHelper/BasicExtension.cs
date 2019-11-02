using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;

namespace GraphQlHelper
{
    public static  class BasicExtension
    {
        public static void BaseMutation<T,TId, TGQlType, TInputType, TCreateType>(this IRepository<T,TId, TCreateType> repository, string name, ObjectGraphType appMutation)
            where T : IBaseElement<TId>
            where TGQlType : IGraphType
            where TInputType : GraphType
        {
            appMutation.Field<TGQlType>(
                $"Create{name}",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<TInputType>>()
                    { Name = name })
                ,
                resolve: context =>
                {
                    var producer = context.GetArgument<TCreateType>(name.ToLower());
                    return repository.Create(producer);
                }
            );

            var idName = name + nameof(IBaseElement.Id);

            appMutation.Field<TGQlType>(
                $"Update{name}",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<TInputType>>() { Name = name },
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = idName }
                )
                ,
                resolve: context =>
                {
                    var producerId = context.GetArgument<TId>(idName);
                    var producer = context.GetArgument<TCreateType>(name.ToLower());

                    var dbProducer = repository.Get(producerId).Result;

                    return repository.Update(dbProducer, producer);
                }
            );

            appMutation.Field<TGQlType>(
                $"Delete{name}",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = idName }
                )
                ,
                resolve: context =>
                {
                    var producerId = context.GetArgument<TId>(idName);

                    var dbProducer = repository.Get(producerId).Result;

                    repository.Delete(dbProducer, true);

                    return $"The element with the id: {producerId} has been successfully deleted from db.";
                }
            );
        }


        public static void BaseQuerry<TQlType, TType, TId>(this IRepository<TType, TId> repository, string nameType, ObjectGraphType obj)
            where TType : IBaseElement<TId>
            where TQlType : IGraphType
        {
            const string start = "start";
            const string count = "count";

            obj.Field<ListGraphType<TQlType>>(
                nameType + "s",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = nameof(IBaseElement.Name) },
                    new QueryArgument<IntGraphType> { Name = start },
                    new QueryArgument<IntGraphType> { Name = count }
                ),
                resolve: context =>
                {
                    var name = context.GetArgument<string>(nameof(IBaseElement.Name).ToLower());
                    var star = context.GetArgument<int?>(start);
                    var coun = context.GetArgument<int?>(count);

                    return repository.Get(name, star, coun);
                });

            obj.Field<TQlType>(
                nameType,
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = nameof(IBaseElement.Id) }),
                resolve: context =>
                {
                    var id = context.GetArgument<TId>(nameof(IBaseElement.Id).ToLower());

                    return repository.Get(id);
                });
        }
    }
}
