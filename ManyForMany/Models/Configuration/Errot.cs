using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.Models.Configuration
{
    public enum Error
    {
        ElementDoseNotExist,
        NotAllowedToSignIn,
        UserNotLogged,
        ListDoseNotContainElement
    }

    public static class Errors
    {
        public const string UserIsAlredyAdded = "User Is Alredy Added To Project";
        public const string UserIsNotAdded = "User Is Not Added To Project";
        public const string UserIsNotExist = "User Is Not Exist";
        public const string UserIsExist = "User Is Exist";



        public const string OrderIsNotExistInList = "Order Is Not Exist In List";
        public const string UserIsNotInterestedOrder = "User Is Not Interested Order";

    }
}
