using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TODOIT.Model.Entity.User;
using TODOIT.ViewModel.Opinion;

namespace TODOIT.Model.Entity.Rate
{
    public class Opinion 
    {
        public Opinion()
        {

        }

        public Opinion(string authorId, Guid orderId, OpinionViewModel model)
        {
            AuthorId = authorId;
            OrderId = orderId;
            this.Assign(model);
        }

        [Key]
        public Guid Id { get; private set; }

        [Required]
        public ApplicationUser Author { get; private set; }

        [ForeignKey(nameof(Author))]
        public string AuthorId { get; private set; }

        [Required]
        public Order.Order Order { get; private set; }

        [ForeignKey(nameof(Order))]
        public Guid OrderId { get; private set; }

        public string Comment { get; set; }

        public Rate Quality { get; set; }

        public Rate Salary { get; set; }
    }
}
