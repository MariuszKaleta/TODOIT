using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ManyForMany.Models.Configuration
{
    public static class AuthorizationHelper
    {
        public const string TokenEndPoint = "/connect/token";

        public const string AbsolutePath = "~";
    }

    public static class SwaggerHelper
    {
        public static string LocalServer = true ? $"/{Assembly.GetExecutingAssembly().GetName().Name}" : "";


        public static string XmlPath = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        public static string JsonPath = $"{LocalServer}/swagger/v1/swagger.json";
    }
}
