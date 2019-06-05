using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizeTester.Model
{
    public static class CustomRoles
    {
        public const string Admin = nameof(Admin);

        public const string BasicUser = nameof(BasicUser);

        public static IEnumerable<string> All
        {
            get
            {
                yield return Admin;
                yield return BasicUser;

            }
        }
    }
}
