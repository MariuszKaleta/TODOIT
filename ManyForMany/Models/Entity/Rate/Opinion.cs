using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ManyForMany.Models.Entity.Order;
using ManyForMany.Models.Entity.User;
using ManyForMany.ViewModel.Opinion;
using ManyForMany.ViewModel.Order;
using ManyForMany.ViewModel.Team;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcHelper.Entity;

namespace ManyForMany.Models.Entity.Rate
{
    public class Opinion : OpinionViewModel
    {
        public Opinion()
        {

        }

        public Opinion(ApplicationUser author, Order.Order order, CreateOpinionViewModel model)
        {
            Author = author;
            Order = order;
        }

        [Key]
        public string Id { get; private set; }

        [Required]
        public ApplicationUser Author { get; private set; }

        public Order.Order Order { get; private set; }
    }

    public static class OpinionExtesnion
    {
        public static ShowOpinionViewModel ToShowOpinionViewModel(this  Opinion opinion, DbSet<Order.Order> orders, ILogger logger)
        {
            var order = orders.Get(opinion.Id, logger).Result;
            
            return new ShowOpinionViewModel(opinion, order);
        }
    }


    public enum Rate
    {
        VeryBad,
        Bad,
        Ok,
        Good, 
        VeryGood
    }
}
