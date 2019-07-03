﻿using System;
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
        public const string UserNameIsBusy = "UserName Is Busy";


        
        public const string OrderIsNotExistInList = "Order Is Not Exist In List";
        public const string UserIsNotInterestedOrder = "User Is Not Interested Order";
        public const string OrderDoseNotExistOrIsNotBelongToYou = "Order Dose Not Exist Or Is Not Belong To You";
        public const string OwnerOfOrderDontLookingForTeam = "Owner of order dont looking for team";
        public const string YouCantJoinToYourOrderTeam = "You Cant Join To Your Order Team";


        public const string SkillIsAlreadyExist = "Skill Is Already Exist";
        public const string SkillIsNotExistInList = "Skill Is Not Exist In List";

        public const string ChatIsNotExist = "Chat Is Not Exist";
        public const string ChatIsExist = "Chat Is Exist";
        public const string ThisIsNotYourChat = "This is not your chat";
        public const string YouDontBelngToChat = "You Dont Belong To Chat";

        public const string YouMustLog = "You Must log";


    }
}
