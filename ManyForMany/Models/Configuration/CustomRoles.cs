using System.Collections.Generic;

namespace ManyForMany.Models.Configuration
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
