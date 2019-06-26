using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.ViewModel.Order
{
    public class CreateOrderViewModel : OrderViewModel
    {
        public int[] RequiredSkills { get; set; }

        public int[] GoodIfHave { get; set; }
    }



}
