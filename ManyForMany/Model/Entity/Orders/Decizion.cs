using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Model.Entity.Ofert;

namespace ManyForMany.Model.Entity.Orders
{
    public class Decizion
    {
        private Decizion()
        {

        }

        public Decizion(Order order, bool decide)
        {
            OrderDecizion = decide;
            Order = order;
        }

        [Key]
        public int Id { get; private set; }

        [Required]
        public bool OrderDecizion { get; private set; }


        [Required]
        public Order Order { get; set; }
    }

    
}
