using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AuthorizeTester.Model
{
    public static class AuthorizationHelper
    {
        public const string TokenEndPoint = "/connect/token";
        public const string AuthorizeEndPoint = "/connect/authorize";
        public const string LogoutEndPoint = "/connect/logout";
        public const string UserInfoEndPoint = "/api/UserInfo";

        public const string AbsolutePath = "~";


    }

    public static class SwaggerHelper
    {

        public static string XmlPath = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        public static string JsonPath = $"/ManyForMany/swagger/v1/swagger.json";
    }
}
