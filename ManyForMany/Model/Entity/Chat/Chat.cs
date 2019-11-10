using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TODOIT.Model.Entity.Chat
{
    public class Chat
    {
        private Chat()
        {

        }


        public Chat( Guid orderId)
        {
            OrderId = orderId;
        }

        [Key]
        public Guid Id { get; private set; }

        [Required]
        public Order.Order Order { get; private set; }

        [Required]
        [ForeignKey(nameof(Order))]
        public Guid OrderId { get; private set; }

    }
}
