using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity.Rate
{
    public class AllowToMakeOpinion
    {
        public AllowToMakeOpinion()
        {

        }

        public AllowToMakeOpinion(Guid orderId, string userId)
        {
            OrderId = orderId;
            UserId = userId;
        }


        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [ForeignKey(nameof(Order))]
        public Guid OrderId { get; set; }
        
        public Order.Order Order { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        
        public ApplicationUser User { get; set; }
    }
}
