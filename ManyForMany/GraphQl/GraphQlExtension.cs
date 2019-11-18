using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Server.Authorization.AspNetCore;
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
        public static string GetUserId(this IDictionary<string, object> dictionary)
        {
            if (!dictionary.TryGetValue(ClaimTypes.NameIdentifier, out var id))
            {
                throw new Exception(Errors.ToUseThisYouMustBeLogged);
            }

            return (string) id;
        }
    }

}