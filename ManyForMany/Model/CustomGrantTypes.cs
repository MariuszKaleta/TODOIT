﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace AuthorizeTester.Model
{
    public static class CustomGrantTypes
    {
        public static string ToPermission(this string grantType)
        {
            return $"{OpenIddictConstants.Permissions.Prefixes.GrantType}{grantType}";
        }

        public const string Google = nameof(Google);

        public static void AddCustomGrantTypes(this OpenIddictServerBuilder options)
        {
            foreach (var grantType in All)
            {
                options.AllowCustomFlow(grantType);
            }
        }

        public static IEnumerable<string> All 
        {
            get
            {
                yield return Google;
            }
        }
    }
}
