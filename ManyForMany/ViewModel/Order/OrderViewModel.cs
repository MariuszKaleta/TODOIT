using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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

        [DateInRange(MustBeAfterToday = true, CanBeNull = true)]
        public DateTime DeadLine { get; set; }
    }
}
