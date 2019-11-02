using System;
using System.ComponentModel.DataAnnotations;
using TODOIT.Model.Entity.User;
using TODOIT.ViewModel.Opinion;

namespace TODOIT.Model.Entity.Rate
{
    public class Opinion 
    {
        public Opinion()
        {

        }

        public Opinion(ApplicationUser author, Order.Order order, OpinionViewModel model)
        {
            Author = author;
            Order = order;

            this.Assign(model);
        }

        [Key]
        public Guid Id { get; private set; }

        [Required]
        public ApplicationUser Author { get; private set; }


        public Order.Order Order { get; private set; }

        public string Comment { get; set; }

        public Rate Quality { get; set; }

        public Rate Salary { get; set; }
    }
}
