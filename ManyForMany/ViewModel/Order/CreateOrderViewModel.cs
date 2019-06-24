using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyForMany.ViewModel.Order
{
    public class CreateOrderViewModel : OrderViewModel
    {
        public List<int> RequiredSkills { get; set; }

        public List<int> GoodIfHave { get; set; }
    }



}
