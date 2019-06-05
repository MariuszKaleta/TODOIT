using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Model.Entity.Orders;
using ManyForMany.ViewModel.Order;
using MultiLanguage.Validation.Attributes;

namespace ManyForMany.Model.Entity.Ofert
{
    public class Order 
    {
        private Order()
        {

        }

        public Order(OrderViewModel model )
        {
            Title = model.Title;
            Describe = model.Describe;
        }


        [Key]
        public int Id { get; private set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Describe { get; set; }

    }
}
