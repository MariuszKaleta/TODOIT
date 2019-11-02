using System;
using TODOIT.Model.Entity.Order;

namespace TODOIT.ViewModel.Order
{
    public class OrderViewModel 
    {
        public string Name { get; set; }

        public string Describe { get; set; }

        [MvcHelper.Validation.Attributes.DateInRange(CanBeNull = true,MustBeAfterToday = true)]
        public DateTime DeadLine { get; set; }

        public OrderStatus OrderStatus { get; set; }

        //[Required]
        //public int[] Categories { get; set; }
    }
}