using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TODOIT.GraphQl.Queries;
using TODOIT.Model.Entity;
using TODOIT.Repositories;
using TODOIT.Repositories.Contracts;

namespace TODOIT.Controller
{
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]

    [ApiController]
    public class GraphQLController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IOpinionRepository _opinionRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;

        public GraphQLController(IOrderRepository orderRepository, IUserRepository userRepository, ISkillRepository skillRepository, IOpinionRepository opinionRepository, IChatRepository chatRepository, IMessageRepository messageRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _skillRepository = skillRepository;
            _opinionRepository = opinionRepository;
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
        }

        [MvcHelper.Attributes.HttpPost("graphql")]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            var inputs = query.Variables.ToInputs();

            var schema = new Schema()
            {
                Query = new AppQuery(_orderRepository, _userRepository, _skillRepository, _opinionRepository, _chatRepository, _messageRepository)
            };

            var result = await new EfDocumentExecuter().ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = query.Query;
                _.OperationName = query.OperationName;
                _.Inputs = inputs;
            });

            if (result.Errors?.Count > 0)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }

    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }
    }
}
