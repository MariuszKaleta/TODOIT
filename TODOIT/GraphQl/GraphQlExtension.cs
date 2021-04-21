using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using TODOIT.Model.Configuration;
using Field = GraphQL.Instrumentation.Field;

namespace TODOIT.GraphQl
{
    public static class GraphQlExtension
    {
        public static string GetUserId(this object value)
        {
            if (value == null || !(value is IDictionary<object, object> dictionary))
            {
                throw new Exception(Errors.ToUseThisYouMustBeLogged);
            }


            if (!dictionary.TryGetValue(ClaimTypes.NameIdentifier, out var id))
            {
                throw new Exception(Errors.ToUseThisYouMustBeLogged);
            }

            return (string) id;
        }
    }

}