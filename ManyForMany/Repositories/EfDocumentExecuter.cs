using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Language.AST;
using OpenIddict.Validation;

namespace TODOIT.Repositories
{
    public class EfDocumentExecuter : DocumentExecuter
    {


        protected override IExecutionStrategy SelectExecutionStrategy(ExecutionContext context)
        {
            if (context.Operation.OperationType == OperationType.Query)
            {
                return new SerialExecutionStrategy();
            }

            return base.SelectExecutionStrategy(context);
        }
    }
}
