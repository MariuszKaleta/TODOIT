using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity.Order
{
    public class OrderMember
    {
        private OrderMember()
        {

        }

        public OrderMember(string userId, Guid orderId)
        {
            UserId = userId;
            OrderId = orderId;
        }

        [Key]
        public Guid Id { get; private set; }

        [Required]
        public ApplicationUser User { get; private set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; private set; }

        [Required]
        public Order Order { get; private set; }

        [ForeignKey(nameof(Order))]
        public Guid OrderId { get; private set; }
    }
}
