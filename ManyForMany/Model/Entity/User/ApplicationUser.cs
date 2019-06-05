using System.Collections.Generic;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.Entity.Orders;
using Microsoft.AspNetCore.Identity;

namespace ManyForMany.Model.Entity.User
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
        }

        #region Proeprties

        public List<Decizion>  DecidedOrders { get; private set; }

        public List<Order> UserOrders { get; private set; }

        #endregion
    }
}