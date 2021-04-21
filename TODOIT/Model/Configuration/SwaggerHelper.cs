using System.Reflection;

namespace TODOIT.Model.Configuration
{
    public static class SwaggerHelper
    {
        public static string LocalServer = false ? $"/{Assembly.GetExecutingAssembly().GetName().Name}" : "";


        public static string XmlPath = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        public static string JsonPath = $"{LocalServer}/swagger/v1/swagger.json";
    }
}