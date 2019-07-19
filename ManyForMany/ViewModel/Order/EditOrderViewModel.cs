using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Entity.Order;

namespace ManyForMany.ViewModel.Order
{
    public class EditOrderViewModel : OrderViewModel
    {
        public OrderStatus Status { get; set; }
    }
}
