using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MultiLanguage;

namespace ManyForMany.Controller
{
    [MvcHelper.Attributes.Route(MvcHelper.AttributeHelper.Api, MvcHelper.AttributeHelper.Controller)]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet(nameof(Errors))]
        public (string, int)[] Errors()
        {
            return MultiLanguageError.ErrorCode.ToArray();
        }
    }
}