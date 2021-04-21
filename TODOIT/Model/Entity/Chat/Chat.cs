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


        public Chat( Guid id)
        {
            Id = id;
        }


        [Required]
        public Order.Order Order { get; private set; }

        [Required]
        [Key]
        [ForeignKey(nameof(Order))]
        public Guid Id { get; private set; }

    }
}
