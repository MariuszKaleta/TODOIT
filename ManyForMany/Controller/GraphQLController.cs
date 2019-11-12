using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TODOIT.GraphQl.Queries;
using TODOIT.GraphQl.Schema;
using TODOIT.Model.Entity;
using TODOIT.Repositories;
using TODOIT.Repositories.Contracts;
using Microsoft.Extensions.DependencyInjection;
using TODOIT.Model.Configuration;

namespace TODOIT.Controller
{
    [ApiController]
    public class GraphQLController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IServiceProvider _serviceProvider;


        public GraphQLController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }

        //[Authorize]
        [MvcHelper.Attributes.HttpPost(AuthorizationHelper.AbsolutePath + "/graphql")]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            var inputs = query.Variables.ToInputs();

            var schema = new AppSchema(_serviceProvider);

            var executor = _serviceProvider.GetService<IDocumentExecuter>();

            var userContext = User.Claims.ToDictionary(x => x.Type, x => (object) x.Value);

            var result = await executor.ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = query.Query;
                _.OperationName = query.OperationName;
                _.Inputs = inputs;
                _.UserContext = userContext;
            });

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }
    }

    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }
    }
}
