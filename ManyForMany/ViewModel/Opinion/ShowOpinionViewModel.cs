using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Entity.Order;
using ManyForMany.Models.Entity.Rate;
using ManyForMany.ViewModel.Order;

namespace ManyForMany.ViewModel.Opinion
{
    public class ShowOpinionViewModel : OpinionViewModel
    {
        public ShowOpinionViewModel(Models.Entity.Rate.Opinion opinion, Models.Entity.Order.Order order)
        {
            Order = order.ToThumbnailInformation();
            Quality = opinion.Quality;
            Salary = opinion.Salary;
            Text = opinion.Text;

        }

        public ThumbnailOrderViewModel Order { get; set; }
    }
}
