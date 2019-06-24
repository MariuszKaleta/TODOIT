using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Model.Entity.Ofert;
using ManyForMany.Model.Entity.Orders;
using ManyForMany.Model.File;
using ManyForMany.Models.Entity.Order;
using MultiLanguage.Validation.Attributes;
using MvcHelper.Validation.Attributes;

namespace ManyForMany.ViewModel.Order
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {

        }


        [Required]
        public string Title { get; set; }

        [Required]
        public string Describe { get; set; }

        public Image[] Images { get; set; }

        [DateInRange(MustBeAfterToday = true, CanBeNull = true)]
        public DateTime DeadLine { get; set; }
    }
}
