using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.ViewModel.Order
{
    public class Decide
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public bool Decision { get; set; }
    }
}
