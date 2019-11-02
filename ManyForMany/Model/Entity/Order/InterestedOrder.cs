using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TODOIT.Model.Entity.User;

namespace TODOIT.Model.Entity.Order
{
    public class InterestedOrder
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public Order Order { get; set; }
    }
}
