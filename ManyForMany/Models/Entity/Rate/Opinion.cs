using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using ManyForMany.ViewModel.Team;

namespace ManyForMany.Models.Entity.Rate
{
    public class Opinion
    {
        public Opinion()
        {

        }

        public Opinion(ApplicationUser author, Model.Entity.Ofert.Order order, OpinionViewModel model)
        {
            Author = author;
            Order = order;
        }

        [Key]
        public string Id { get; private set; }

        public ApplicationUser Author { get; private set; }

        public Model.Entity.Ofert.Order Order { get; private set; }

        public string Text { get; private set; }

        public Rate Quality { get; private set; }

        public Rate Salary { get; private set; }
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
