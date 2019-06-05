using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Model.Entity.Orders;
using MultiLanguage.Validation.Attributes;

namespace ManyForMany.ViewModel.Order
{
    public class OrderViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Describe { get; set; }

        [NotEmpty]
        public Image[] Images { get; set; }
    }
}
