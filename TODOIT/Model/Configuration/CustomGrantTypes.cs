using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace TODOIT.Model.Configuration
{
    public static class CustomGrantTypes
    {
        public static string ToPermission(this string grantType)
        {
            return $"{OpenIddictConstants.Permissions.Prefixes.GrantType}{grantType}";
        }

        public static void AddCustomGrantTypes(this OpenIddictServerBuilder builder)
        {
            foreach (var grantType in All)
            {
                builder.AllowCustomFlow(grantType);
            }
        }

        public const string Google = nameof(Google);
        public const string Linkedin = nameof(Linkedin);
        public const string Facebook = nameof(Facebook);
        /*
        public static void AddCustomGrantTypes(this OpenIddictServerBuilder options)
        {
            foreach (var grantType in All)
            {
                options.AllowCustomFlow(grantType);
            }
        }
        */
        public static IEnumerable<string> All 
        {
            get
            {
                yield return Google;
                yield return Linkedin;
                yield return Facebook;
            }
        }
    }
}
