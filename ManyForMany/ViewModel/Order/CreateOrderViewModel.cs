using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ManyForMany.ViewModel.Order
{
    public class CreateOrderViewModel : OrderViewModel
    {
        public FileViewModel[] Images { get; set; }

        public FileViewModel[] Files { get; set; }

        public int[] RequiredSkills { get; set; }

        public int[] GoodIfHave { get; set; }
    }



}
